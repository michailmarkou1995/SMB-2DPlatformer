using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Abilities.Pickups;
using Core.NPC;
using Core.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private const float loadSceneDelay = 1f;

        public bool hurryUp; // within last 100 secs?
        public int marioSize; // 0..2
        public int lives;
        public int coins;
        public int scores;
        public float timeLeft;
        private int _timeLeftInt;

        private bool _isRespawning;
        private bool _isPoweringDown;

        public bool isInvinciblePowerdown;
        public bool isInvincibleStarman;
        private const float MarioInvinciblePowerdownDuration = 2;
        private const float MarioInvincibleStarmanDuration = 12;
        private const float TransformDuration = 1;

        private GameStateManager _gameStateManager;
        private PlayerController _playerController;
        private Animator _marioAnimator;
        private Rigidbody2D _marioRigidbody2D;

        public Text scoreText;
        public Text coinText;
        public Text timeText;
        public GameObject floatingTextEffect;
        private const float FloatingTextOffsetY = 2f;

        public AudioSource musicSource;
        public AudioSource soundSource;
        public AudioSource pauseSoundSource;

        public AudioClip levelMusic;
        public AudioClip levelMusicHurry;
        public AudioClip starmanMusic;
        public AudioClip starmanMusicHurry;
        public AudioClip levelCompleteMusic;
        public AudioClip castleCompleteMusic;

        public AudioClip oneUpSound;
        public AudioClip bowserFallSound;
        public AudioClip bowserFireSound;
        public AudioClip breakBlockSound;
        public AudioClip bumpSound;
        public AudioClip coinSound;
        public AudioClip deadSound;
        public AudioClip fireballSound;
        public AudioClip flagpoleSound;
        public AudioClip jumpSmallSound;
        public AudioClip jumpSuperSound;
        public AudioClip kickSound;
        public AudioClip pipePowerdownSound;
        public AudioClip powerupSound;
        public AudioClip powerupAppearSound;
        public AudioClip stompSound;
        public AudioClip warningSound;

        public int coinBonus = 200;
        public int powerupBonus = 1000;
        public int starmanBonus = 1000;
        public int oneupBonus = 0;
        public int breakBlockBonus = 50;

        public Vector2 stompBounceVelocity = new Vector2(0, 15);

        public bool gamePaused;
        public bool timerPaused;
        public bool musicPaused;


        private void Awake()
        {
            Time.timeScale = 1;
        }

        private void Start()
        {
            _gameStateManager = FindObjectOfType<GameStateManager>();
            RetrieveGameState();

            _playerController = FindObjectOfType<PlayerController>();
            _marioAnimator = _playerController.gameObject.GetComponent<Animator>();
            _marioRigidbody2D = _playerController.gameObject.GetComponent<Rigidbody2D>();
            _playerController.UpdateSize();

            // Sound volume
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
            soundSource.volume = PlayerPrefs.GetFloat("soundVolume");
            pauseSoundSource.volume = PlayerPrefs.GetFloat("soundVolume");

            // HUD
            SetHudCoin();
            SetHudScore();
            SetHudTime();
            ChangeMusic(hurryUp ? levelMusicHurry : levelMusic);

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        /****NEW STUFF FOR EVENT SYSTEM***/
        private void OnEnable()
        {
            Coin.OnCoinCollected += AddCoin;
            Starman.OnStarmanCollected += MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected += AddLife;
            PowerupObject.OnPowerUpCollected += MarioPowerUp;
        }

        private void OnDisable()
        {
            Coin.OnCoinCollected -= AddCoin;
            Starman.OnStarmanCollected -= MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected -= AddLife;
            PowerupObject.OnPowerUpCollected -= MarioPowerUp;
        }
        /****NEW STUFF FOR EVENT SYSTEM END HERE***/

        private void RetrieveGameState()
        {
            marioSize = _gameStateManager.marioSize;
            lives = _gameStateManager.lives;
            coins = _gameStateManager.coins;
            scores = _gameStateManager.scores;
            timeLeft = _gameStateManager.timeLeft;
            hurryUp = _gameStateManager.hurryUp;
        }


        /****************** Timer */
        private void Update()
        {
            if (!timerPaused) {
                timeLeft -= Time.deltaTime; // / .4f; // 1 game sec ~ 0.4 real time sec
                SetHudTime();
            }

            if (_timeLeftInt < 100 && !hurryUp) {
                hurryUp = true;
                PauseMusicPlaySound(warningSound, true);
                ChangeMusic(isInvincibleStarman ? starmanMusicHurry : levelMusicHurry, warningSound.length);
            }

            if (_timeLeftInt <= 0) {
                MarioRespawn(true);
            }

            if (!Input.GetButtonDown("Pause")) return;
            StartCoroutine(!gamePaused ? PauseGameCo() : UnpauseGameCo());
        }


        /****************** Game pause */
        List<Animator> unscaledAnimators = new List<Animator>();
        private float _pauseGamePrevTimeScale;
        private bool _pausePrevMusicPaused;
        private static readonly int IsInvincibleStarman = Animator.StringToHash("isInvincibleStarman");
        private static readonly int IsInvinciblePowerdown = Animator.StringToHash("isInvinciblePowerdown");
        private static readonly int IsPoweringUp = Animator.StringToHash("isPoweringUp");
        private static readonly int IsPoweringDown = Animator.StringToHash("isPoweringDown");

        private IEnumerator PauseGameCo()
        {
            gamePaused = true;
            _pauseGamePrevTimeScale = Time.timeScale;

            Time.timeScale = 0;
            _pausePrevMusicPaused = musicPaused;
            musicSource.Pause();
            musicPaused = true;
            soundSource.Pause();

            // Set any active animators that use unscaled time mode to normal
            unscaledAnimators.Clear();
            foreach (Animator animator in FindObjectsOfType<Animator>()) {
                if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
                unscaledAnimators.Add(animator);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            pauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(pauseSoundSource.clip.length);
            Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + _pauseGamePrevTimeScale.ToString());
        }

        private IEnumerator UnpauseGameCo()
        {
            pauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(pauseSoundSource.clip.length);

            musicPaused = _pausePrevMusicPaused;
            if (!musicPaused) {
                musicSource.UnPause();
            }

            soundSource.UnPause();

            // Reset animators
            foreach (Animator animator in unscaledAnimators) {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            unscaledAnimators.Clear();

            Time.timeScale = _pauseGamePrevTimeScale;
            gamePaused = false;
            Debug.Log(this.name + " UnpauseGameCo stops: resume prevTimeScale=" + _pauseGamePrevTimeScale.ToString());
        }


        /****************** Invincibility */
        public bool IsInvincible()
        {
            return isInvinciblePowerdown || isInvincibleStarman;
        }

        private void MarioInvincibleStarman()
        {
            StartCoroutine(MarioInvincibleStarmanCo());
            AddScore(starmanBonus, _playerController.transform.position);
        }

        IEnumerator MarioInvincibleStarmanCo()
        {
            isInvincibleStarman = true;
            _marioAnimator.SetBool(IsInvincibleStarman, true);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario After Starman");
            ChangeMusic(hurryUp ? starmanMusicHurry : starmanMusic);

            yield return new WaitForSeconds(MarioInvincibleStarmanDuration);
            isInvincibleStarman = false;
            _marioAnimator.SetBool(IsInvincibleStarman, false);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario");
            ChangeMusic(hurryUp ? levelMusicHurry : levelMusic);
        }

        void MarioInvinciblePowerdown()
        {
            StartCoroutine(MarioInvinciblePowerdownCo());
        }

        IEnumerator MarioInvinciblePowerdownCo()
        {
            isInvinciblePowerdown = true;
            _marioAnimator.SetBool(IsInvinciblePowerdown, true);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario After Powerdown");
            yield return new WaitForSeconds(MarioInvinciblePowerdownDuration);
            isInvinciblePowerdown = false;
            _marioAnimator.SetBool(IsInvinciblePowerdown, false);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario");
        }


        /****************** Powerup / Powerdown / Die */
        private void MarioPowerUp()
        {
            soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
            if (marioSize < 2) {
                StartCoroutine(MarioPowerUpCo());
            }

            AddScore(powerupBonus, _playerController.transform.position);
        }

        private IEnumerator MarioPowerUpCo()
        {
            _marioAnimator.SetBool(IsPoweringUp, true);
            Time.timeScale = 0f;
            _marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => gamePaused);

            Time.timeScale = 1;
            _marioAnimator.updateMode = AnimatorUpdateMode.Normal;

            marioSize++;
            _playerController.UpdateSize();
            _marioAnimator.SetBool(IsPoweringUp, false);
        }

        public void MarioPowerDown()
        {
            if (!_isPoweringDown) {
                Debug.Log(this.name + " MarioPowerDown: called and executed");
                _isPoweringDown = true;

                if (marioSize > 0) {
                    StartCoroutine(MarioPowerDownCo());
                    soundSource.PlayOneShot(pipePowerdownSound);
                } else {
                    MarioRespawn();
                }

                Debug.Log(this.name + " MarioPowerDown: done executing");
            } else {
                Debug.Log(this.name + " MarioPowerDown: called but not executed");
            }
        }

        private IEnumerator MarioPowerDownCo()
        {
            _marioAnimator.SetBool(IsPoweringDown, true);
            Time.timeScale = 0f;
            _marioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => gamePaused);

            Time.timeScale = 1;
            _marioAnimator.updateMode = AnimatorUpdateMode.Normal;
            MarioInvinciblePowerdown();

            marioSize = 0;
            _playerController.UpdateSize();
            _marioAnimator.SetBool(IsPoweringDown, false);
            _isPoweringDown = false;
        }

        public void MarioRespawn(bool timeUp = false)
        {
            if (_isRespawning) return;
            _isRespawning = true;

            marioSize = 0;
            lives--;

            soundSource.Stop();
            musicSource.Stop();
            musicPaused = true;
            soundSource.PlayOneShot(deadSound);

            Time.timeScale = 0f;
            _playerController.FreezeAndDie();

            if (timeUp) {
                Debug.Log(this.name + " MarioRespawn: called due to timeup");
            }

            Debug.Log(this.name + " MarioRespawn: lives left=" + lives.ToString());

            if (lives > 0) {
                ReloadCurrentLevel(deadSound.length, timeUp);
            } else {
                LoadGameOver(deadSound.length, timeUp);
                Debug.Log(this.name + " MarioRespawn: all dead");
            }
        }


        /****************** Kill enemy */
        public void MarioStompEnemy(Enemy enemy)
        {
            _marioRigidbody2D.velocity =
                new Vector2(_marioRigidbody2D.velocity.x + stompBounceVelocity.x, stompBounceVelocity.y);
            enemy.StompedByMario();
            soundSource.PlayOneShot(stompSound);
            AddScore(enemy.stompBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStompEnemy called on " + enemy.gameObject.name);
        }

        public void MarioStarmanTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByStarmanMario();
            soundSource.PlayOneShot(kickSound);
            AddScore(enemy.starmanBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStarmanTouchEnemy called on " + enemy.gameObject.name);
        }

        public void RollingShellTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByRollingShell();
            soundSource.PlayOneShot(kickSound);
            AddScore(enemy.rollingShellBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " RollingShellTouchEnemy called on " + enemy.gameObject.name);
        }

        public void BlockHitEnemy(Enemy enemy)
        {
            enemy.HitBelowByBlock();
            AddScore(enemy.hitByBlockBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " BlockHitEnemy called on " + enemy.gameObject.name);
        }

        public void FireballTouchEnemy(Enemy enemy)
        {
            enemy.HitByMarioFireball();
            soundSource.PlayOneShot(kickSound);
            AddScore(enemy.fireballBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " FireballTouchEnemy called on " + enemy.gameObject.name);
        }

        /****************** Scene loading */
        private void LoadSceneDelay(string sceneName, float delay = loadSceneDelay)
        {
            timerPaused = true;
            StartCoroutine(LoadSceneDelayCo(sceneName, delay));
        }

        private IEnumerator LoadSceneDelayCo(string sceneName, float delay)
        {
            Debug.Log(this.name + " LoadSceneDelayCo: starts loading " + sceneName);

            float waited = 0;
            while (waited < delay) {
                if (!gamePaused) {
                    // should not count delay while game paused
                    waited += Time.unscaledDeltaTime;
                }

                yield return null;
            }

            yield return new WaitWhile(() => gamePaused);

            Debug.Log(this.name + " LoadSceneDelayCo: done loading " + sceneName);

            _isRespawning = false;
            _isPoweringDown = false;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadNewLevel(string sceneName, float delay = loadSceneDelay)
        {
            _gameStateManager.SaveGameState();
            _gameStateManager.ConfigNewLevel();
            _gameStateManager.sceneToLoad = sceneName;
            LoadSceneDelay("Level Start Screen", delay);
        }

        public void LoadSceneCurrentLevel(string sceneName, float delay = loadSceneDelay)
        {
            _gameStateManager.SaveGameState();
            _gameStateManager.ResetSpawnPosition(); // TODO
            LoadSceneDelay(sceneName, delay);
        }

        public void LoadSceneCurrentLevelSetSpawnPipe(string sceneName, int spawnPipeIdx, float delay = loadSceneDelay)
        {
            _gameStateManager.SaveGameState();
            _gameStateManager.SetSpawnPipe(spawnPipeIdx);
            LoadSceneDelay(sceneName, delay);
            Debug.Log(this.name + " LoadSceneCurrentLevelSetSpawnPipe: supposed to load " + sceneName
                      + ", spawnPipeIdx=" + spawnPipeIdx.ToString() + "; actual GSM spawnFromPoint="
                      + _gameStateManager.spawnFromPoint.ToString() + ", spawnPipeIdx="
                      + _gameStateManager.spawnPipeIdx.ToString());
        }

        private void ReloadCurrentLevel(float delay = loadSceneDelay, bool timeup = false)
        {
            _gameStateManager.SaveGameState();
            _gameStateManager.ConfigReplayedLevel();
            _gameStateManager.sceneToLoad = SceneManager.GetActiveScene().name;
            LoadSceneDelay(timeup ? "Time Up Screen" : "Level Start Screen", delay);
        }

        private void LoadGameOver(float delay = loadSceneDelay, bool timeup = false)
        {
            int currentHighScore = PlayerPrefs.GetInt("highScore", 0);
            if (scores > currentHighScore) {
                PlayerPrefs.SetInt("highScore", scores);
            }

            _gameStateManager.timeUp = timeup;
            LoadSceneDelay("Game Over Screen", delay);
        }


        /****************** HUD and sound effects */
        private void SetHudCoin()
        {
            coinText.text = "x" + coins.ToString("D2");
        }

        private void SetHudScore()
        {
            scoreText.text = scores.ToString("D6");
        }

        private void SetHudTime()
        {
            _timeLeftInt = Mathf.RoundToInt(timeLeft);
            timeText.text = _timeLeftInt.ToString("D3");
        }

        private void CreateFloatingText(string text, Vector3 spawnPos)
        {
            GameObject textEffect = Instantiate(floatingTextEffect, spawnPos, Quaternion.identity);
            textEffect.GetComponentInChildren<TextMesh>().text = text.ToUpper();
        }


        private void ChangeMusic(AudioClip clip, float delay = 0)
        {
            StartCoroutine(ChangeMusicCo(clip, delay));
        }

        private IEnumerator ChangeMusicCo(AudioClip clip, float delay)
        {
            Debug.Log(this.name + " ChangeMusicCo: starts changing music to " + clip.name);
            musicSource.clip = clip;
            yield return new WaitWhile(() => gamePaused);
            yield return new WaitForSecondsRealtime(delay);
            yield return new WaitWhile(() => gamePaused || musicPaused);
            if (!_isRespawning) {
                musicSource.Play();
            }

            Debug.Log(this.name + " ChangeMusicCo: done changing music to " + clip.name);
        }

        private void PauseMusicPlaySound(AudioClip clip, bool resumeMusic)
        {
            StartCoroutine(PauseMusicPlaySoundCo(clip, resumeMusic));
        }

        private IEnumerator PauseMusicPlaySoundCo(AudioClip clip, bool resumeMusic)
        {
            string musicClipName = "";
            if (musicSource.clip) {
                musicClipName = musicSource.clip.name;
            }

            Debug.Log(this.name + " PausemusicPlaySoundCo: starts pausing music " + musicClipName + " to play sound " +
                      clip.name);

            musicPaused = true;
            musicSource.Pause();
            soundSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
            if (resumeMusic) {
                musicSource.UnPause();

                musicClipName = "";
                if (musicSource.clip) {
                    musicClipName = musicSource.clip.name;
                }

                Debug.Log(this.name + " PausemusicPlaySoundCo: resume playing music " + musicClipName);
            }

            musicPaused = false;

            Debug.Log(this.name + " PausemusicPlaySoundCo: done pausing music to play sound " + clip.name);
        }

        /****************** Game state */
        public void AddLife()
        {
            lives++;
            soundSource.PlayOneShot(oneUpSound);
        }

        public void AddLife(Vector3 spawnPos)
        {
            lives++;
            soundSource.PlayOneShot(oneUpSound);
            CreateFloatingText("1UP", spawnPos);
        }

        public void AddCoin()
        {
            coins++;
            soundSource.PlayOneShot(coinSound);
            if (coins == 100) {
                AddLife();
                coins = 0;
            }

            SetHudCoin();
            AddScore(coinBonus);
        }

        public void AddCoin(Vector3 spawnPos)
        {
            coins++;
            soundSource.PlayOneShot(coinSound);
            if (coins == 100) {
                AddLife();
                coins = 0;
            }

            SetHudCoin();
            AddScore(coinBonus, spawnPos);
        }

        public void AddScore(int bonus)
        {
            scores += bonus;
            SetHudScore();
        }

        public void AddScore(int bonus, Vector3 spawnPos)
        {
            scores += bonus;
            SetHudScore();
            if (bonus > 0) {
                CreateFloatingText(bonus.ToString(), spawnPos);
            }
        }


        /****************** Misc */
        public Vector3 FindSpawnPosition()
        {
            Vector3 spawnPosition;
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            Debug.Log(this.name + " FindSpawnPosition: GSM spawnFromPoint=" +
                      gameStateManager.spawnFromPoint.ToString()
                      + " spawnPipeIdx= " + gameStateManager.spawnPipeIdx.ToString()
                      + " spawnPointIdx=" + gameStateManager.spawnPointIdx.ToString());
            if (gameStateManager.spawnFromPoint) {
                spawnPosition = GameObject.Find("Spawn Points").transform.GetChild(gameStateManager.spawnPointIdx)
                    .transform.position;
            } else {
                spawnPosition = GameObject.Find("Spawn Pipes").transform.GetChild(gameStateManager.spawnPipeIdx)
                    .transform.Find("Spawn Pos").transform.position;
            }

            return spawnPosition;
        }

        public string GetWorldName(string sceneName)
        {
            string[] sceneNameParts = Regex.Split(sceneName, " - ");
            return sceneNameParts[0];
        }

        public bool IsSceneInCurrentWorld(string sceneName)
        {
            return GetWorldName(sceneName) == GetWorldName(SceneManager.GetActiveScene().name);
        }

        public void MarioCompleteCastle()
        {
            timerPaused = true;
            ChangeMusic(castleCompleteMusic);
            musicSource.loop = false;
            _playerController.AutomaticWalk(_playerController.CastleWalkSpeedX);
        }

        public void MarioCompleteLevel()
        {
            timerPaused = true;
            ChangeMusic(levelCompleteMusic);
            musicSource.loop = false;
        }

        public void MarioReachFlagPole()
        {
            timerPaused = true;
            PauseMusicPlaySound(flagpoleSound, false);
            _playerController.ClimbFlagPole();
        }
    }
}
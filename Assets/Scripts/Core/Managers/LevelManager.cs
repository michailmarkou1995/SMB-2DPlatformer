using System.Collections;
using System.Text.RegularExpressions;
using Abilities.Pickups;
using Core.NPC;
using Core.Player;
using Interfaces.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    [RequireComponent(typeof(ISoundManagerExtras))]
    public class LevelManager : LevelManagerBase, ILevelManager
    {
        private IGameStateManager _gameStateManager;
        private ISoundManagerExtras _soundManager;
        private PlayerController _playerController; //TODO IPlayerController
        public ISoundManagerExtras GetSoundManager => _soundManager;

        private void Awake()
        {
            _soundManager = GetComponent<ISoundManagerExtras>();
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
            GetSoundManager.MusicSource.volume = PlayerPrefs.GetFloat("musicVolume");
            GetSoundManager.SoundSource.volume = PlayerPrefs.GetFloat("soundVolume");
            GetSoundManager.PauseSoundSource.volume = PlayerPrefs.GetFloat("soundVolume");

            // HUD
            SetHudCoin();
            SetHudScore();
            SetHudTime();
            ChangeMusic(hurryUp ? GetSoundManager.LevelMusicHurry : GetSoundManager.LevelMusic);

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
            marioSize = _gameStateManager.PlayerSize;
            lives = _gameStateManager.Lives;
            coins = _gameStateManager.Coins;
            scores = _gameStateManager.Scores;
            timeLeft = _gameStateManager.TimeLeft;
            hurryUp = _gameStateManager.HurryUp;
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
                PauseMusicPlaySound(GetSoundManager.WarningSound, true);
                ChangeMusic(isInvincibleStarman ? GetSoundManager.StarmanMusicHurry : GetSoundManager.LevelMusicHurry,
                    GetSoundManager.WarningSound.length);
            }

            if (_timeLeftInt <= 0) {
                MarioRespawn(true);
            }

            if (!Input.GetButtonDown("Pause")) return;
            StartCoroutine(!gamePaused ? PauseGameCo() : UnpauseGameCo());
        }


        /****************** Game pause */

        private IEnumerator PauseGameCo()
        {
            gamePaused = true;
            _pauseGamePrevTimeScale = Time.timeScale;

            Time.timeScale = 0;
            _pausePrevMusicPaused = musicPaused;
            GetSoundManager.MusicSource.Pause();
            musicPaused = true;
            GetSoundManager.SoundSource.Pause();

            // Set any active animators that use unscaled time mode to normal
            _unScaledAnimators.Clear();
            foreach (Animator animator in FindObjectsOfType<Animator>()) {
                if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
                _unScaledAnimators.Add(animator);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);
            Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + _pauseGamePrevTimeScale.ToString());
        }

        private IEnumerator UnpauseGameCo()
        {
            GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);

            musicPaused = _pausePrevMusicPaused;
            if (!musicPaused) {
                GetSoundManager.MusicSource.UnPause();
            }

            GetSoundManager.SoundSource.UnPause();

            // Reset animators
            foreach (Animator animator in _unScaledAnimators) {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            _unScaledAnimators.Clear();

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
            ChangeMusic(hurryUp ? GetSoundManager.StarmanMusicHurry : GetSoundManager.StarmanMusic);

            yield return new WaitForSeconds(MarioInvincibleStarmanDuration);
            isInvincibleStarman = false;
            _marioAnimator.SetBool(IsInvincibleStarman, false);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario");
            ChangeMusic(hurryUp ? GetSoundManager.LevelMusicHurry : GetSoundManager.LevelMusic);
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
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager
                .PowerupSound); // should play sound regardless of size
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
                    GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.PipePowerdownSound);
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
            AnimatorUpdateMode updateMode = _marioAnimator.updateMode;
            updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => gamePaused);

            Time.timeScale = 1;
            updateMode = AnimatorUpdateMode.Normal;
            _marioAnimator.updateMode = updateMode;
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

            GetSoundManager.SoundSource.Stop();
            GetSoundManager.MusicSource.Stop();
            musicPaused = true;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.DeadSound);

            Time.timeScale = 0f;
            _playerController.FreezeAndDie();

            if (timeUp) {
                Debug.Log(this.name + " MarioRespawn: called due to timeup");
            }

            Debug.Log(this.name + " MarioRespawn: lives left=" + lives.ToString());

            if (lives > 0) {
                ReloadCurrentLevel(GetSoundManager.DeadSound.length, timeUp);
            } else {
                LoadGameOver(GetSoundManager.DeadSound.length, timeUp);
                Debug.Log(this.name + " MarioRespawn: all dead");
            }
        }


        /****************** Kill enemy */
        public void MarioStompEnemy(Enemy enemy)
        {
            _marioRigidbody2D.velocity =
                new Vector2(_marioRigidbody2D.velocity.x + stompBounceVelocity.x, stompBounceVelocity.y);
            enemy.StompedByMario();
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.StompSound);
            AddScore(enemy.stompBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStompEnemy called on " + enemy.gameObject.name);
        }

        public void MarioStarmanTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByStarmanMario();
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.KickSound);
            AddScore(enemy.starmanBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStarmanTouchEnemy called on " + enemy.gameObject.name);
        }

        public void RollingShellTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByRollingShell();
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.KickSound);
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
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.KickSound);
            AddScore(enemy.fireballBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " FireballTouchEnemy called on " + enemy.gameObject.name);
        }

        /****************** Scene loading */
        private void LoadSceneDelay(string sceneName, float delay = LoadSceneDelayTime)
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

        public void LoadNewLevel(string sceneName, float delay = LoadSceneDelayTime)
        {
            _gameStateManager.GetSaveGameState();
            _gameStateManager.ConfigNewLevel();
            _gameStateManager.SceneToLoad = sceneName;
            LoadSceneDelay("Level Start Screen", delay);
        }

        public void LoadSceneCurrentLevel(string sceneName, float delay = LoadSceneDelayTime)
        {
            _gameStateManager.GetSaveGameState();
            _gameStateManager.ResetSpawnPosition(); // TODO
            LoadSceneDelay(sceneName, delay);
        }

        public void LoadSceneCurrentLevelSetSpawnPipe(string sceneName, int spawnPipeIdx,
            float delay = LoadSceneDelayTime)
        {
            _gameStateManager.GetSaveGameState();
            _gameStateManager.SetSpawnPipe(spawnPipeIdx);
            LoadSceneDelay(sceneName, delay);
            Debug.Log(this.name + " LoadSceneCurrentLevelSetSpawnPipe: supposed to load " + sceneName
                      + ", spawnPipeIdx=" + spawnPipeIdx.ToString() + "; actual GSM spawnFromPoint="
                      + _gameStateManager.SpawnFromPoint.ToString() + ", spawnPipeIdx="
                      + _gameStateManager.SpawnPipeIdx.ToString());
        }

        private void ReloadCurrentLevel(float delay = LoadSceneDelayTime, bool timeUp = false)
        {
            _gameStateManager.GetSaveGameState();
            _gameStateManager.ConfigReplayedLevel();
            _gameStateManager.SceneToLoad = SceneManager.GetActiveScene().name;
            LoadSceneDelay(timeUp ? "Time Up Screen" : "Level Start Screen", delay);
        }

        private void LoadGameOver(float delay = LoadSceneDelayTime, bool timeup = false)
        {
            int currentHighScore = PlayerPrefs.GetInt("highScore", 0);
            if (scores > currentHighScore) {
                PlayerPrefs.SetInt("highScore", scores);
            }

            _gameStateManager.TimeUp = timeup;
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
            GetSoundManager.MusicSource.clip = clip;
            yield return new WaitWhile(() => gamePaused);
            yield return new WaitForSecondsRealtime(delay);
            yield return new WaitWhile(() => gamePaused || musicPaused);
            if (!_isRespawning) {
                GetSoundManager.MusicSource.Play();
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
            if (GetSoundManager.MusicSource.clip) {
                musicClipName = GetSoundManager.MusicSource.clip.name;
            }

            Debug.Log(this.name + " Pause musicPlaySoundCo: starts pausing music " + musicClipName + " to play sound " +
                      clip.name);

            musicPaused = true;
            GetSoundManager.MusicSource.Pause();
            GetSoundManager.SoundSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
            if (resumeMusic) {
                GetSoundManager.MusicSource.UnPause();

                musicClipName = "";
                if (GetSoundManager.MusicSource.clip) {
                    musicClipName = GetSoundManager.MusicSource.clip.name;
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
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.OneUpSound);
        }

        public void AddLife(Vector3 spawnPos)
        {
            lives++;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.OneUpSound);
            CreateFloatingText("1UP", spawnPos);
        }

        public void AddCoin()
        {
            coins++;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.CoinSound);
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
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.CoinSound);
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
                      gameStateManager.SpawnFromPoint.ToString()
                      + " spawnPipeIdx= " + gameStateManager.SpawnPipeIdx.ToString()
                      + " spawnPointIdx=" + gameStateManager.SpawnPointIdx.ToString());
            if (gameStateManager.SpawnFromPoint) {
                spawnPosition = GameObject.Find("Spawn Points").transform.GetChild(gameStateManager.SpawnPointIdx)
                    .transform.position;
            } else {
                spawnPosition = GameObject.Find("Spawn Pipes").transform.GetChild(gameStateManager.SpawnPipeIdx)
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
            ChangeMusic(GetSoundManager.CastleCompleteMusic);
            GetSoundManager.MusicSource.loop = false;
            _playerController.AutomaticWalk(_playerController.CastleWalkSpeedX);
        }

        public void MarioCompleteLevel()
        {
            timerPaused = true;
            ChangeMusic(GetSoundManager.LevelCompleteMusic);
            GetSoundManager.MusicSource.loop = false;
        }

        public void MarioReachFlagPole()
        {
            timerPaused = true;
            PauseMusicPlaySound(GetSoundManager.FlagpoleSound, false);
            _playerController.ClimbFlagPole();
        }
    }
}
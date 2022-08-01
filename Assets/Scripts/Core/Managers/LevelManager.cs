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
            MarioAnimator = _playerController.gameObject.GetComponent<Animator>();
            MarioRigidbody2D = _playerController.gameObject.GetComponent<Rigidbody2D>();
            _playerController.UpdateSize();

            GetSoundManager.GetSoundVolume();

            // HUD
            SetHudCoin();
            SetHudScore();
            SetHudTime();
            ChangeMusic(hurryUp ? GetSoundManager.LevelMusicHurry : GetSoundManager.LevelMusic);

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

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

        public void RetrieveGameState()
        {
            marioSize = _gameStateManager.PlayerSize;
            lives = _gameStateManager.Lives;
            coins = _gameStateManager.Coins;
            scores = _gameStateManager.Scores;
            timeLeft = _gameStateManager.TimeLeft;
            hurryUp = _gameStateManager.HurryUp;
        }

        private void Update()
        {
            if (!timerPaused) {
                timeLeft -= Time.deltaTime; // / .4f; // 1 game sec ~ 0.4 real time sec
                SetHudTime();
            }

            if (TimeLeftInt < 100 && !hurryUp) {
                hurryUp = true;
                PauseMusicPlaySound(GetSoundManager.WarningSound, true);
                ChangeMusic(isInvincibleStarman ? GetSoundManager.StarmanMusicHurry : GetSoundManager.LevelMusicHurry,
                    GetSoundManager.WarningSound.length);
            }

            if (TimeLeftInt <= 0) {
                MarioRespawn(true);
            }

            if (!Input.GetButtonDown("Pause")) return;
            StartCoroutine(!gamePaused ? PauseGameCo() : UnpauseGameCo());
        }


        /****************** Game pause */

        private IEnumerator PauseGameCo()
        {
            gamePaused = true;
            PauseGamePrevTimeScale = Time.timeScale;

            Time.timeScale = 0;
            PausePrevMusicPaused = musicPaused;
            GetSoundManager.MusicSource.Pause();
            musicPaused = true;
            GetSoundManager.SoundSource.Pause();

            // Set any active animators that use unscaled time mode to normal
            UnScaledAnimators.Clear();
            foreach (Animator animator in FindObjectsOfType<Animator>()) {
                if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
                UnScaledAnimators.Add(animator);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);
            Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + PauseGamePrevTimeScale.ToString());
        }

        private IEnumerator UnpauseGameCo()
        {
            GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);

            musicPaused = PausePrevMusicPaused;
            if (!musicPaused) {
                GetSoundManager.MusicSource.UnPause();
            }

            GetSoundManager.SoundSource.UnPause();

            // Reset animators
            foreach (Animator animator in UnScaledAnimators) {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            UnScaledAnimators.Clear();

            Time.timeScale = PauseGamePrevTimeScale;
            gamePaused = false;
            Debug.Log(this.name + " UnpauseGameCo stops: resume prevTimeScale=" + PauseGamePrevTimeScale.ToString());
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
            MarioAnimator.SetBool(IsInvincibleStarmanAnim, true);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario After Starman");
            ChangeMusic(hurryUp ? GetSoundManager.StarmanMusicHurry : GetSoundManager.StarmanMusic);

            yield return new WaitForSeconds(MarioInvincibleStarmanDuration);
            isInvincibleStarman = false;
            MarioAnimator.SetBool(IsInvincibleStarmanAnim, false);
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
            MarioAnimator.SetBool(IsInvinciblePowerdownAnim, true);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario After Powerdown");
            yield return new WaitForSeconds(MarioInvinciblePowerdownDuration);
            isInvinciblePowerdown = false;
            MarioAnimator.SetBool(IsInvinciblePowerdownAnim, false);
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
            MarioAnimator.SetBool(IsPoweringUpAnim, true);
            Time.timeScale = 0f;
            MarioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => gamePaused);

            Time.timeScale = 1;
            MarioAnimator.updateMode = AnimatorUpdateMode.Normal;

            marioSize++;
            _playerController.UpdateSize();
            MarioAnimator.SetBool(IsPoweringUpAnim, false);
        }

        public void MarioPowerDown()
        {
            if (!IsPoweringDown) {
                Debug.Log(this.name + " MarioPowerDown: called and executed");
                IsPoweringDown = true;

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
            MarioAnimator.SetBool(IsPoweringDownAnim, true);
            Time.timeScale = 0f;
            AnimatorUpdateMode updateMode = MarioAnimator.updateMode;
            updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => gamePaused);

            Time.timeScale = 1;
            updateMode = AnimatorUpdateMode.Normal;
            MarioAnimator.updateMode = updateMode;
            MarioInvinciblePowerdown();

            marioSize = 0;
            _playerController.UpdateSize();
            MarioAnimator.SetBool(IsPoweringDownAnim, false);
            IsPoweringDown = false;
        }

        public void MarioRespawn(bool timeUp = false)
        { //TODO make every one class
            if (IsRespawning) return;
            IsRespawning = true;

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
            MarioRigidbody2D.velocity =
                new Vector2(MarioRigidbody2D.velocity.x + stompBounceVelocity.x, stompBounceVelocity.y);
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

            IsRespawning = false;
            IsPoweringDown = false;
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
            TimeLeftInt = Mathf.RoundToInt(timeLeft);
            timeText.text = TimeLeftInt.ToString("D3");
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
            if (!IsRespawning) {
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
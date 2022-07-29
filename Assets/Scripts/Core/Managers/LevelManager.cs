using System.Collections;
using System.Text.RegularExpressions;
using Abilities.Pickups;
using Core.NPC;
using Core.Player;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using Interfaces.UI;

namespace Core.Managers
{
    [RequireComponent(typeof(ISoundManagerExtras))]
    [RequireComponent(typeof(ISoundLevelHandle))]
    [RequireComponent(typeof(IGameStateManager))]
    [RequireComponent(typeof(ILoadLevelSceneHandle))]
    [RequireComponent(typeof(IHUD))]
    public class LevelManager : LevelManagerBase, ILevelManager
    {
        #region GettersAndSetters

        // public bool GamePaused
        // {
        //     get => gamePaused;
        //     set => gamePaused = value;
        // }
        //
        // public bool TimerPaused
        // {
        //     get => timerPaused;
        //     set => timerPaused = value;
        // }
        //
        // public bool MusicPaused
        // {
        //     get => musicPaused;
        //     set => musicPaused = value;
        // }

        public bool IsRespawning
        {
            get => isRespawning;
            set => isRespawning = value;
        }

        public bool IsPoweringDown
        {
            get => isPoweringDown;
            set => isPoweringDown = value;
        }

        public ISoundManagerExtras GetSoundManager => _soundManager;
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler => _loadLevelSceneHandler;
        public IGameStateManager GetGameStateManager => _gameStateManager;
        public IHUD GetHUD => _hud;

        #endregion

        private IGameStateManager _gameStateManager;
        private ISoundManagerExtras _soundManager;
        private ISoundLevelHandle _soundLevelHandler;
        private PlayerController _playerController; //TODO IPlayerController
        private ILoadLevelSceneHandle _loadLevelSceneHandler;
        private IHUD _hud;

        private void Awake()
        {
            _soundManager = GetComponent<ISoundManagerExtras>();
            _soundLevelHandler = GetComponent<ISoundLevelHandle>();
            _loadLevelSceneHandler = GetComponent<ILoadLevelSceneHandle>();
            _hud = GetComponent<IHUD>();

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

            _hud.SetHUD();
            _soundLevelHandler.ChangeMusic(hurryUp ? GetSoundManager.LevelMusicHurry : GetSoundManager.LevelMusic);

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
            _hud.Coins = _gameStateManager.Coins;
            _hud.Scores = _gameStateManager.Scores;
            _hud.TimeLeft = _gameStateManager.TimeLeft;
            hurryUp = _gameStateManager.HurryUp;
        }

        private void Update()
        {
            if (!_gameStateManager.TimerPaused) {
                _hud.TimeLeft -= Time.deltaTime; // / .4f; // 1 game sec ~ 0.4 real time sec
                _hud.SetHudTime();
            }

            if (_hud.TimeLeftInt < 100 && !hurryUp) {
                hurryUp = true;
                _soundLevelHandler.PauseMusicPlaySound(GetSoundManager.WarningSound, true);
                _soundLevelHandler.ChangeMusic(
                    isInvincibleStarman ? GetSoundManager.StarmanMusicHurry : GetSoundManager.LevelMusicHurry,
                    GetSoundManager.WarningSound.length);
            }

            if (_hud.TimeLeftInt <= 0) {
                MarioRespawn(true);
            }

            if (!Input.GetButtonDown("Pause")) return;
            _gameStateManager.PauseUnPauseState();
        }

        // private void PauseUnpauseState()
        // {
        //     StartCoroutine(!GamePaused ? PauseGameCo() : UnpauseGameCo());
        // }
        //
        //
        // /****************** Game pause */
        //
        // private IEnumerator PauseGameCo()
        // {
        //     GamePaused = true;
        //     PauseGamePrevTimeScale = Time.timeScale;
        //
        //     Time.timeScale = 0;
        //     PausePrevMusicPaused = MusicPaused;
        //     GetSoundManager.MusicSource.Pause();
        //     MusicPaused = true;
        //     GetSoundManager.SoundSource.Pause();
        //
        //     // Set any active animators that use unscaled time mode to normal
        //     UnScaledAnimators.Clear();
        //     foreach (Animator animator in FindObjectsOfType<Animator>()) {
        //         if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
        //         UnScaledAnimators.Add(animator);
        //         animator.updateMode = AnimatorUpdateMode.Normal;
        //     }
        //
        //     GetSoundManager.PauseSoundSource.Play();
        //     yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);
        //     Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + PauseGamePrevTimeScale.ToString());
        // }
        //
        // private IEnumerator UnpauseGameCo()
        // {
        //     GetSoundManager.PauseSoundSource.Play();
        //     yield return new WaitForSecondsRealtime(GetSoundManager.PauseSoundSource.clip.length);
        //
        //     MusicPaused = PausePrevMusicPaused;
        //     if (!MusicPaused) {
        //         GetSoundManager.MusicSource.UnPause();
        //     }
        //
        //     GetSoundManager.SoundSource.UnPause();
        //
        //     // Reset animators
        //     foreach (Animator animator in UnScaledAnimators) {
        //         animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        //     }
        //
        //     UnScaledAnimators.Clear();
        //
        //     Time.timeScale = PauseGamePrevTimeScale;
        //     GamePaused = false;
        //     Debug.Log(this.name + " UnpauseGameCo stops: resume prevTimeScale=" + PauseGamePrevTimeScale.ToString());
        // }


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
            _soundLevelHandler.ChangeMusic(hurryUp ? GetSoundManager.StarmanMusicHurry : GetSoundManager.StarmanMusic);

            yield return new WaitForSeconds(MarioInvincibleStarmanDuration);
            isInvincibleStarman = false;
            MarioAnimator.SetBool(IsInvincibleStarmanAnim, false);
            _playerController.gameObject.layer = LayerMask.NameToLayer("Mario");
            _soundLevelHandler.ChangeMusic(hurryUp ? GetSoundManager.LevelMusicHurry : GetSoundManager.LevelMusic);
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
            yield return new WaitWhile(() => _gameStateManager.GamePaused);

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
            MarioAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => _gameStateManager.GamePaused);

            Time.timeScale = 1;
            MarioAnimator.updateMode = AnimatorUpdateMode.Normal;
            MarioInvinciblePowerdown();

            marioSize = 0;
            _playerController.UpdateSize();
            MarioAnimator.SetBool(IsPoweringDownAnim, false);
            isPoweringDown = false;
        }

        public void MarioRespawn(bool timeUp = false)
        {
            //TODO make every one class
            if (IsRespawning) return;
            IsRespawning = true;

            marioSize = 0;
            lives--;

            GetSoundManager.SoundSource.Stop();
            GetSoundManager.MusicSource.Stop();
            _gameStateManager.MusicPaused = true;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.DeadSound);

            Time.timeScale = 0f;
            _playerController.FreezeAndDie();

            if (timeUp) {
                Debug.Log(this.name + " MarioRespawn: called due to timeup");
            }

            Debug.Log(this.name + " MarioRespawn: lives left=" + lives.ToString());

            if (lives > 0) {
                _loadLevelSceneHandler.ReloadCurrentLevel(GetSoundManager.DeadSound.length, timeUp);
            } else {
                _loadLevelSceneHandler.LoadGameOver(GetSoundManager.DeadSound.length, timeUp);
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
            _hud.CreateFloatingText("1UP", spawnPos);
        }

        public void AddCoin()
        {
            _hud.Coins++;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.CoinSound);
            if (_hud.Coins == 100) {
                AddLife();
                _hud.Coins = 0;
            }

            _hud.SetHudCoin();
            AddScore(coinBonus);
        }

        public void AddCoin(Vector3 spawnPos)
        {
            _hud.Coins++;
            GetSoundManager.SoundSource.PlayOneShot(GetSoundManager.CoinSound);
            if (_hud.Coins == 100) {
                AddLife();
                _hud.Coins = 0;
            }

            _hud.SetHudCoin();
            AddScore(coinBonus, spawnPos);
        }

        public void AddScore(int bonus)
        {
            _hud.Scores += bonus;
            _hud.SetHudScore();
        }

        public void AddScore(int bonus, Vector3 spawnPos)
        {
            _hud.Scores += bonus;
            _hud.SetHudScore();
            if (bonus > 0) {
                _hud.CreateFloatingText(bonus.ToString(), spawnPos);
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
            _gameStateManager.TimerPaused = true;
            _soundLevelHandler.ChangeMusic(GetSoundManager.CastleCompleteMusic);
            GetSoundManager.MusicSource.loop = false;
            _playerController.AutomaticWalk(_playerController.CastleWalkSpeedX);
        }

        public void MarioCompleteLevel()
        {
            _gameStateManager.TimerPaused = true;
            _soundLevelHandler.ChangeMusic(GetSoundManager.LevelCompleteMusic);
            GetSoundManager.MusicSource.loop = false;
        }

        public void MarioReachFlagPole()
        {
            _gameStateManager.TimerPaused = true;
            _soundLevelHandler.PauseMusicPlaySound(GetSoundManager.FlagpoleSound, false);
            _playerController.ClimbFlagPole();
        }
    }

    public class LevelPowerUps : MonoBehaviour //, IPowerUps
    { }

    public class LevelState : MonoBehaviour { }

    public class LevelKillEnemy : MonoBehaviour { }

    public class LevelPlayerInvincibility : MonoBehaviour { }

    public class LevelDie : MonoBehaviour { }
}
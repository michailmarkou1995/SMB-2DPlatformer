using Abilities.Pickups;
using Core.Player;
using Interfaces.Abilities.Pickups;
using Interfaces.Abilities.Player;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using Interfaces.UI;

namespace Core.Managers
{
    //[RequireComponent(typeof(ISoundManagerExtras))]
    //[RequireComponent(typeof(ISoundLevelHandle))]
    [RequireComponent(typeof(IGameStateManager))]
    [RequireComponent(typeof(ILoadLevelSceneHandle))]
    [RequireComponent(typeof(IHUD))]
    [RequireComponent(typeof(IPlayerPickUpAbilities))]
    [RequireComponent(typeof(IPlayerAbilities))]
    public class LevelManager : LevelManagerBase, ILevelManager
    {
        #region GettersAndSetters

        // Exposed API's Calls External
        public ISoundManagerExtras GetSoundManager => _soundManager;
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler => _loadLevelSceneHandler;
        public IGameStateManager GetGameStateManager => _gameStateManager;
        public IHUD GetHUD => _hud;
        public IPlayerPickUpAbilities GetPlayerPickUpAbilities => _playerPickUpAbilities;
        public IPlayerAbilities GetPlayerAbilities => _playerAbilities;

        public PlayerController GetPlayerController => _playerController;

        //public ISoundLevelHandle GetSoundLevelHandle => _soundLevelHandler;
        public ILevelServices GetLevelServices => _levelServices;

        public bool TimerPaused
        {
            get => timerPaused;
            set => timerPaused = value;
        }
        
        public bool GamePaused
        {
            get => gamePaused;
            set => gamePaused = value;
        }
        
        public bool MusicPaused
        {
            get => musicPaused;
            set => musicPaused = value;
        }

        #endregion

        private IGameStateManager _gameStateManager;

        private ISoundManagerExtras _soundManager;

        //private ISoundLevelHandle _soundLevelHandler;
        private PlayerController _playerController; //TODO IPlayerController
        private ILoadLevelSceneHandle _loadLevelSceneHandler;
        private IHUD _hud;
        private IPlayerPickUpAbilities _playerPickUpAbilities;
        private IPlayerAbilities _playerAbilities;
        private ILevelServices _levelServices;

        private void Awake()
        {
            _soundManager = FindObjectOfType<SoundManager>();
            //_soundManager = GetComponent<ISoundManagerExtras>();
            //_soundLevelHandler = GetComponent<ISoundLevelHandle>();
            _loadLevelSceneHandler = GetComponent<ILoadLevelSceneHandle>();
            _gameStateManager = FindObjectOfType<GameStateManager>();
            _hud = GetComponent<IHUD>();
            _playerPickUpAbilities = GetComponent<IPlayerPickUpAbilities>();
            _playerAbilities = GetComponent<IPlayerAbilities>();
            _levelServices = GetComponent<ILevelServices>();

            Time.timeScale = 1;
        }

        private void Start()
        {
            RetrieveGameState();

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        private void OnEnable()
        {
            Coin.OnCoinCollected += _playerPickUpAbilities.AddCoin;
            Starman.OnStarmanCollected += _playerAbilities.MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected += _playerPickUpAbilities.AddLife;
            PowerupObject.OnPowerUpCollected += _playerAbilities.MarioPowerUp;
        }

        private void OnDisable()
        {
            Coin.OnCoinCollected -= _playerPickUpAbilities.AddCoin;
            Starman.OnStarmanCollected -= _playerAbilities.MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected -= _playerPickUpAbilities.AddLife;
            PowerupObject.OnPowerUpCollected -= _playerAbilities.MarioPowerUp;
        }

        public void RetrieveGameState()
        {
            // Lives = _gameStateManager.Lives;
            _hud.Coins = _gameStateManager.Coins;
            _hud.Scores = _gameStateManager.Scores;
            _hud.TimeLeft = _gameStateManager.TimeLeft;
            // marioSize = _gameStateManager.PlayerSize;
            // HurryUp = _gameStateManager.HurryUp;

            _playerController = FindObjectOfType<PlayerController>();
            PlayerAnimator.PlayerAnimatorComponent = _playerController.gameObject.GetComponent<Animator>();
            _playerAbilities.PlayerRigidbody2D = _playerController.gameObject.GetComponent<Rigidbody2D>();
            _playerController.UpdateSize();

            GetSoundManager.GetSoundVolume();

            _hud.SetHUD();
            _soundManager.GetSoundLevelHandle.ChangeMusic(_gameStateManager.HurryUp
                ? GetSoundManager.LevelMusicHurry
                : GetSoundManager.LevelMusic);
        }

        private void Update()
        {
            // _gameStateManager.TimerHUD();
            //
            // _soundManager.GetSoundLevelHandle.TimerHUDMusic();
            //
            // _gameStateManager.TimeUpCounter();
            //
            // _gameStateManager.GamePauseCheck();
            TimerHUD();

            TimerHUDMusic();

            TimeUpCounter();

            GamePauseCheck();
        }
        public void TimerHUD()
        {
            if (_gameStateManager.TimerPaused) return;
            _hud.TimeLeft -= Time.deltaTime; // / .4f; // 1 game sec ~ 0.4 real time sec
            _hud.SetHudTime();
        }
        public void GamePauseCheck()
        {
            if (!Input.GetButtonDown("Pause")) return;
            _gameStateManager.PauseUnPauseState();
        }
        public void TimeUpCounter()
        {
            if (_hud.TimeLeftInt <= 0) {
                _playerAbilities.MarioRespawn(true);
            }
        }
        public void TimerHUDMusic()
        {
            if (_hud.TimeLeftInt >= 100 || _gameStateManager.HurryUp) return;
            _gameStateManager.HurryUp = true;
            _soundManager.GetSoundLevelHandle.PauseMusicPlaySound(_soundManager.WarningSound, true);
            _soundManager.GetSoundLevelHandle.ChangeMusic(
                _playerAbilities.IsInvincibleStarman
                    ? _soundManager.StarmanMusicHurry
                    : _soundManager.LevelMusicHurry,
                _soundManager.WarningSound.length);
        }
    }
}
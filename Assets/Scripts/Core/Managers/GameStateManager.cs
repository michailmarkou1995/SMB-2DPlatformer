using System;
using System.Collections;
using Interfaces.Core.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Managers
{
    [RequireComponent(typeof(ISaveGameState))]
    public class GameStateManager : GameStateManagerBase, IGameStateManager
    {
        #region GettersAndSettersFromBase

        public bool SpawnFromPoint
        {
            get => spawnFromPoint;
            set => spawnFromPoint = value;
        }

        public int SpawnPointIdx
        {
            get => spawnPointIdx;
            set => spawnPointIdx = value;
        }

        public int SpawnPipeIdx
        {
            get => spawnPipeIdx;
            set => spawnPipeIdx = value;
        }

        public int PlayerSize
        {
            get => playerSize;
            set => playerSize = value;
        }

        public int Lives
        {
            get => lives;
            set => lives = value;
        }

        public int Coins
        {
            get => coins;
            set => coins = value;
        }

        public int Scores
        {
            get => scores;
            set => scores = value;
        }

        public float TimeLeft
        {
            get => timeLeft;
            set => timeLeft = value;
        }

        public int TimeLeftInt
        {
            get => timeLeftInt;
            set => timeLeftInt = value;
        }

        public bool HurryUp
        {
            get => hurryUp;
            set => hurryUp = value;
        }

        public string SceneToLoad
        {
            get => sceneToLoad;
            set => sceneToLoad = value;
        }

        public bool TimeUp
        {
            get => timeUp;
            set => timeUp = value;
        }

        public bool GamePaused
        {
            get => gamePaused;
            set => gamePaused = value;
        }

        public bool TimerPaused
        {
            get => timerPaused;
            set => timerPaused = value;
        }

        public bool MusicPaused
        {
            get => musicPaused;
            set => musicPaused = value;
        }

        #endregion

        private ISaveGameState _saveGameStateOnMemory;
        [CanBeNull] private ILevelManager _levelManager;

        private void Awake()
        {
            RetainGameStateManagerPerLoad();
            _saveGameStateOnMemory = GetComponent<ISaveGameState>();
        }

        /// <summary>
        /// DontDestroyOnLoad is used to keep the GameStateManager alive between scenes.
        /// </summary>
        public void RetainGameStateManagerPerLoad()
        {
            if (FindObjectsOfType(GetType()).Length == 1) {
                DontDestroyOnLoad(gameObject);
                ConfigNewGame();
            } else {
                Destroy(gameObject);
            }
        }

        public void ResetSpawnPosition()
        {
            SpawnFromPoint = true;
            SpawnPointIdx = 0;
            SpawnPipeIdx = 0;
        }

        public void SetSpawnPipe(int idx)
        {
            SpawnFromPoint = false;
            SpawnPipeIdx = idx;
        }

        public void ConfigNewGame()
        {
            PlayerSize = 0;
            Lives = 3;
            Coins = 0;
            Scores = 0;
            TimeLeft = 400.5f;
            HurryUp = false;
            ResetSpawnPosition();
            SceneToLoad = null;
            TimeUp = false;
        }

        public void ConfigNewLevel()
        {
            TimeLeft = 400.5f;
            HurryUp = false;
            ResetSpawnPosition();
        }

        public void ConfigReplayedLevel()
        {
            // e.g. Mario respawns
            TimeLeft = 400.5f;
            HurryUp = false;
        }

        public void GetSaveGameState()
        {
            _saveGameStateOnMemory.SaveGameState(this);
        }

        public void PauseUnPauseState()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            StartCoroutine(!GamePaused ? PauseGameCo() : UnpauseGameCo());
        }

        private IEnumerator PauseGameCo()
        {
            if (_levelManager == null) {
                yield break;
            }

            Debug.Log("PauseGameCo!!!!!!!!!!");
            GamePaused = true;
            PauseGamePrevTimeScale = Time.timeScale;

            Time.timeScale = 0;
            PausePrevMusicPaused = MusicPaused;
            _levelManager.GetSoundManager.MusicSource.Pause();
            MusicPaused = true;
            _levelManager.GetSoundManager.SoundSource.Pause();

            // Set any active animators that use unscaled time mode to normal
            UnScaledAnimators.Clear();
            foreach (Animator animator in FindObjectsOfType<Animator>()) {
                if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
                UnScaledAnimators.Add(animator);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            _levelManager.GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(_levelManager.GetSoundManager.PauseSoundSource.clip.length);
            Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + PauseGamePrevTimeScale.ToString());
        }

        private IEnumerator UnpauseGameCo()
        {
            if (_levelManager == null) {
                yield break;
            }

            _levelManager.GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(_levelManager.GetSoundManager.PauseSoundSource.clip.length);

            MusicPaused = PausePrevMusicPaused;
            if (!MusicPaused) {
                _levelManager.GetSoundManager.MusicSource.UnPause();
            }

            _levelManager.GetSoundManager.SoundSource.UnPause();

            // Reset animators
            foreach (Animator animator in UnScaledAnimators) {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            UnScaledAnimators.Clear();

            Time.timeScale = PauseGamePrevTimeScale;
            GamePaused = false;
            Debug.Log(this.name + " UnpauseGameCo stops: resume prevTimeScale=" + PauseGamePrevTimeScale.ToString());
        }
    }
}
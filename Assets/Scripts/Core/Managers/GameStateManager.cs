using System;
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
        
        public bool HurryUp
        {
            get => hurryUp;
            set => hurryUp = value;
        }

        #endregion

        private ISaveGameState _saveGameStateOnMemory;

        [CanBeNull] private Interfaces.Core.Managers.ILevelManager _levelManager;

        private void Awake()
        {
            base.Awake();
            ConfigNewGame();
            //RetainGameStateManagerPerLoad();
            _saveGameStateOnMemory = GetComponent<ISaveGameState>();
        }

        /// <summary>
        /// DontDestroyOnLoad is used to keep the GameStateManager alive between scenes.
        /// </summary>
        [Obsolete("Not used anymore use base.Awake instead.", true)]
        public void RetainGameStateManagerPerLoad()
        {
            if (FindObjectsOfType(GetType()).Length == 1) {
                DontDestroyOnLoad(gameObject);
                ConfigNewGame();
            } else if (FindObjectsOfType(GetType()).Length > 1) {
                Destroy(gameObject);
            } else {
                Debug.LogError("No GameStateManager found");
            }
        }

        public void ResetSpawnPosition()
        {
            spawnFromPoint = true;
            spawnPointIdx = 0;
            spawnPipeIdx = 0;
        }

        public void SetSpawnPipe(int idx)
        {
            spawnFromPoint = false;
            spawnPipeIdx = idx;
        }

        public void ConfigNewGame()
        {
            playerSize = 0;
            lives = 3;
            coins = 0;
            scores = 0;
            timeLeft = 400.5f;
            hurryUp = false;
            ResetSpawnPosition();
            sceneToLoad = null;
            timeUp = false;
        }

        public void ConfigNewLevel()
        {
            timeLeft = 400.5f;
            hurryUp = false;
            ResetSpawnPosition();
            //TimerPaused = false;
        }

        public void ConfigReplayedLevel()
        {
            // e.g. Mario respawns
            timeLeft = 400.5f;
            hurryUp = false;
        }

        public void GetSaveGameState()
        {
            _saveGameStateOnMemory.SaveGameState(this);
        }
    }
}
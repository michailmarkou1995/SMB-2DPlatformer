using Interfaces.Core.Managers;
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

        #endregion

        private ISaveGameState _saveGameStateOnMemory;

        private void Awake()
        {
            RetainGameStateManagerPerLoad();
            _saveGameStateOnMemory = GetComponent<ISaveGameState>();
        }

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
    }
}
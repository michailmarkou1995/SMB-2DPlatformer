using UI.Interfaces;
using UnityEngine;

namespace Core.Managers
{
    public class GameStateManager : MonoBehaviour, IGameStateManager
    {
        public bool spawnFromPoint;
        public int spawnPointIdx;
        public int spawnPipeIdx;

        public int marioSize;
        public int lives;
        public int coins;
        public int scores;
        public float timeLeft;
        public bool hurryUp;

        public string sceneToLoad; // what scene to load after level start screen finishes?
        public bool timeUp;

        private void Awake()
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
            marioSize = 0;
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
        }

        public void ConfigReplayedLevel()
        {
            // e.g. Mario respawns
            timeLeft = 400.5f;
            hurryUp = false;
        }

        public void SaveGameState()
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            marioSize = levelManager.marioSize;
            lives = levelManager.lives;
            coins = levelManager.coins;
            scores = levelManager.scores;
            timeLeft = levelManager.timeLeft;
            hurryUp = levelManager.hurryUp;
        }
    }
}
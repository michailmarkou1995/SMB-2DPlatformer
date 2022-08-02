using System.Collections;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LevelSceneHandle : MonoBehaviour, ILoadLevelSceneHandle
    {
        private const float LoadSceneDelayTime = 1f;

        private ILevelManager _levelManager;
        
        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public void LoadSceneDelay(string sceneName, float delay = LoadSceneDelayTime)
        {
            _levelManager.GetGameStateData.TimerPaused = true;
            StartCoroutine(LoadSceneDelayCo(sceneName, delay));
        }

        private IEnumerator LoadSceneDelayCo(string sceneName, float delay)
        {
            Debug.Log(this.name + " LoadSceneDelayCo: starts loading " + sceneName);

            float waited = 0;
            while (waited < delay) {
                if (!_levelManager.GetGameStateData.GamePaused) {
                    // should not count delay while game paused
                    waited += Time.unscaledDeltaTime;
                }

                yield return null;
            }

            yield return new WaitWhile(() => _levelManager.GetGameStateData.GamePaused);

            Debug.Log(this.name + " LoadSceneDelayCo: done loading " + sceneName);

            _levelManager.GetPlayerAbilities.IsRespawning = false;
            _levelManager.GetPlayerAbilities.IsPoweringDown = false;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadLevel(string sceneName, float delay = LoadSceneDelayTime)
        {
            IGameStateManager levelManagerGetGameStateManager = _levelManager.GetGameStateManager;
            levelManagerGetGameStateManager.GetSaveGameState();
            levelManagerGetGameStateManager.ConfigNewLevel();
            levelManagerGetGameStateManager.SceneToLoad = sceneName;
            LoadSceneDelay("Level Start Screen", delay);
        }

        public void LoadSceneCurrentLevel(string sceneName, float delay = LoadSceneDelayTime)
        {
            IGameStateManager levelManagerGetGameStateManager = _levelManager.GetGameStateManager;
            levelManagerGetGameStateManager.GetSaveGameState();
            levelManagerGetGameStateManager.ResetSpawnPosition(); // TODO
            LoadSceneDelay(sceneName, delay);
        }

        public void LoadSceneCurrentLevelSetSpawnPipe(string sceneName, int spawnPipeIdx,
            float delay = LoadSceneDelayTime)
        {
            IGameStateManager levelManagerGetGameStateManager = _levelManager.GetGameStateManager;
            levelManagerGetGameStateManager.GetSaveGameState();
            levelManagerGetGameStateManager.SetSpawnPipe(spawnPipeIdx);
            LoadSceneDelay(sceneName, delay);
            Debug.Log(this.name + " LoadSceneCurrentLevelSetSpawnPipe: supposed to load " + sceneName
                      + ", spawnPipeIdx=" + spawnPipeIdx.ToString() + "; actual GSM spawnFromPoint="
                      + levelManagerGetGameStateManager.SpawnFromPoint.ToString() + ", spawnPipeIdx="
                      + levelManagerGetGameStateManager.SpawnPipeIdx.ToString());
        }

        public void ReloadCurrentLevel(float delay = LoadSceneDelayTime, bool timeUp = false)
        {
            IGameStateManager levelManagerGetGameStateManager = _levelManager.GetGameStateManager;
            levelManagerGetGameStateManager.GetSaveGameState();
            levelManagerGetGameStateManager.ConfigReplayedLevel();
            levelManagerGetGameStateManager.SceneToLoad = SceneManager.GetActiveScene().name;
            LoadSceneDelay(timeUp ? "Time Up Screen" : "Level Start Screen", delay);
        }

        public void LoadGameOver(float delay = LoadSceneDelayTime, bool timeUp = false)
        {
            int currentHighScore = PlayerPrefs.GetInt("highScore", 0);
            if (_levelManager.GetHUD.Scores > currentHighScore) {
                PlayerPrefs.SetInt("highScore", _levelManager.GetHUD.Scores);
            }

            _levelManager.GetGameStateManager.TimeUp = timeUp;
            LoadSceneDelay("Game Over Screen", delay);
        }
    }
}
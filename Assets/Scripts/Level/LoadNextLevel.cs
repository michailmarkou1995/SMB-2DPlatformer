using System.Collections;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LoadNextLevel : MonoBehaviour, ILoadLevel
    {
        public void LoadLevel(string loadLevelName, float delay = 0)
        {
            StartCoroutine(LoadSceneDelayCo(loadLevelName, delay));
        }

        private static IEnumerator LoadSceneDelayCo(string sceneName, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}
using System.Collections;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LoadMainMenu : MonoBehaviour, ILoadLevel
    {
        //[SerializeField] private string levelNameToLoad;
        public void LoadLevel(string loadLevelName, float delay = 0)
        {
            StartCoroutine(LoadSceneDelayCo("Main Menu", delay));
        }

        private static IEnumerator LoadSceneDelayCo(string sceneName, float delay = 0)
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}
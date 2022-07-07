using UnityEngine;

namespace Core
{
    public class GameStateManager : MonoBehaviour
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
        public bool timeup;
        
        void Awake () {
            if (FindObjectsOfType (GetType ()).Length == 1) {
                DontDestroyOnLoad (gameObject);
                ConfigNewGame ();
            } else {
                Destroy (gameObject);
            }
        }
	
        public void ResetSpawnPosition() {
            spawnFromPoint = true;
            spawnPointIdx = 0;
            spawnPipeIdx = 0;
        }

        public void ConfigNewGame() {
            marioSize = 0;
            lives = 3;
            coins = 0;
            scores = 0;
            timeLeft = 400.5f;
            hurryUp = false;
            ResetSpawnPosition ();
            sceneToLoad = null;
            timeup = false;
        }
    }
}
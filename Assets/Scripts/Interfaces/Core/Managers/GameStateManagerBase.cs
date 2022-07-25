using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class GameStateManagerBase : MonoBehaviour
    {
        public bool spawnFromPoint;
        public int spawnPointIdx;
        public int spawnPipeIdx;

        public int playerSize;
        public int lives;
        public int coins;
        public int scores;
        public float timeLeft;
        public bool hurryUp;

        public string sceneToLoad; // what scene to load after level start screen finishes?
        public bool timeUp;
    }
}
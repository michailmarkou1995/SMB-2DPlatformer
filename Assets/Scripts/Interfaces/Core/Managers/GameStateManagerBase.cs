using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class GameStateManagerBase : MonoBehaviour
    {
        [SerializeField] protected bool spawnFromPoint;
        [SerializeField] protected int spawnPointIdx;
        [SerializeField] protected int spawnPipeIdx;

        [SerializeField] protected int playerSize;
        [SerializeField] protected int lives;
        [SerializeField] protected int coins;
        [SerializeField] protected int scores;
        [SerializeField] protected float timeLeft;
        [SerializeField] protected bool hurryUp;

        [SerializeField] protected string sceneToLoad; // what scene to load after level start screen finishes?
        [SerializeField] protected bool timeUp;
    }
}
using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class GameStateManagerBase : GameStateResetDataBase
    {
        [SerializeField] protected bool spawnFromPoint;
        [SerializeField] protected int spawnPointIdx;
        [SerializeField] protected int spawnPipeIdx;

        [SerializeField] protected string sceneToLoad; // what scene to load after level start screen finishes?
        [SerializeField] protected bool timeUp;
    }
}
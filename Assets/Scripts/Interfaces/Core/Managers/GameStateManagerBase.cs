using System.Collections.Generic;
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
        [SerializeField] protected int timeLeftInt;
        [SerializeField] protected bool hurryUp;

        [SerializeField] protected string sceneToLoad; // what scene to load after level start screen finishes?
        [SerializeField] protected bool timeUp;
        
        [SerializeField] protected bool gamePaused;
        [SerializeField] protected bool timerPaused;
        [SerializeField] protected bool musicPaused;
        
        protected readonly List<Animator> UnScaledAnimators = new List<Animator>();
        protected float PauseGamePrevTimeScale;
        protected bool PausePrevMusicPaused;
    }
}
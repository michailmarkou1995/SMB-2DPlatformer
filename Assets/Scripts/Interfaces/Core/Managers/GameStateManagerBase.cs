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

        [SerializeField] protected bool hurryUp;

        [SerializeField] protected string sceneToLoad; // what scene to load after level start screen finishes?
        [SerializeField] protected bool timeUp;

        [SerializeField] protected int coinBonus = 200;
        [SerializeField] protected int powerupBonus = 1000;
        [SerializeField] protected int starmanBonus = 1000;
        [SerializeField] protected int oneupBonus;
        [SerializeField] protected int breakBlockBonus = 50;
        [SerializeField] protected int timeLeftInt;

        [SerializeField] protected bool gamePaused;
        [SerializeField] protected bool timerPaused;
        [SerializeField] protected bool musicPaused;


        protected readonly List<Animator> UnScaledAnimators = new List<Animator>();
        protected float PauseGamePrevTimeScale;
        protected bool PausePrevMusicPaused;
    }

    // public class GameStateDataBase : MonoBehaviour
    // {
    //     [SerializeField] protected int coinBonus = 200;
    //     [SerializeField] protected int  powerupBonus = 1000;
    //     [SerializeField] protected int  starmanBonus = 1000;
    //     [SerializeField] protected int  oneupBonus;
    //     [SerializeField] protected int  breakBlockBonus = 50;
    //     [SerializeField] protected int timeLeftInt;
    //
    //     
    //     [SerializeField] protected bool gamePaused;
    //     [SerializeField] protected bool timerPaused;
    //     [SerializeField] protected bool musicPaused;
    //     
    //     
    //     protected readonly List<Animator> UnScaledAnimators = new List<Animator>();
    //     protected float PauseGamePrevTimeScale;
    //     protected bool PausePrevMusicPaused;
    // }
    //
    // public class GameStateData: GameStateDataBase, IGameStateData
    // {
    //     public bool CoinBonus { get; set; }
    //     public int PowerupBonus { get; set; }
    //     public int StarmanBonus { get; set; }
    //     public int OneupBonus { get; set; }
    //     public int BreakBlockBonus { get; set; }
    //     public int TimeLeftInt { get; set; }
    //     public int GamePaused { get; set; }
    //     public float TimerPaused { get; set; }
    //     public bool MusicPaused { get; set; }
    //     public string UnScaledAnimators { get; set; }
    //     public bool TimeUp { get; set; }
    // }
    //
    // public interface IGameStateData
    // {
    //     public bool CoinBonus { get; set; }
    //     public int PowerupBonus { get; set; }
    //     public int StarmanBonus { get; set; }
    //     public int OneupBonus { get; set; }
    //     public int BreakBlockBonus { get; set; }
    //     public int TimeLeftInt { get; set; }
    //     public int GamePaused { get; set; }
    //     public float TimerPaused { get; set; }
    //     public bool MusicPaused { get; set; }
    //     public string UnScaledAnimators { get; set; }
    //     public bool TimeUp { get; set; }
    // }
}
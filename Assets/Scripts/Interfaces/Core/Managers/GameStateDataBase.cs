using System.Collections.Generic;
using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class GameStateDataBase : GameStateResetDataBase
    {
        [SerializeField] protected int coinBonus = 200;
        [SerializeField] protected int powerupBonus = 1000;
        [SerializeField] protected int starmanBonus = 1000;
        [SerializeField] protected int oneupBonus;
        [SerializeField] protected int breakBlockBonus = 50;

        [SerializeField] protected bool gamePaused;
        [SerializeField] protected bool timerPaused;
        [SerializeField] protected bool musicPaused;

        protected List<Animator> UnScaledAnimators = new();
        protected float PauseGamePrevTimeScale;
        protected bool PausePrevMusicPaused;
    }
}
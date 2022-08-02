using System.Collections.Generic;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Core.Managers
{
    public class GameStateData : GameStateDataBase, IGameStateData
    {

        #region GettersAndSetters

        public int PlayerSize
        {
            get => playerSize;
            set => playerSize = value;
        }

        public int Lives
        {
            get => lives;
            set => lives = value;
        }

        public int Coins
        {
            get => coins;
            set => coins = value;
        }

        public int Scores
        {
            get => scores;
            set => scores = value;
        }

        public float TimeLeft
        {
            get => timeLeft;
            set => timeLeft = value;
        }

        public bool HurryUp
        {
            get => hurryUp;
            set => hurryUp = value;
        }

        public int CoinBonus
        {
            get => coinBonus;
            set => coinBonus = value;
        }

        public int PowerupBonus
        {
            get => powerupBonus;
            set => powerupBonus = value;
        }

        public int StarmanBonus
        {
            get => starmanBonus;
            set => starmanBonus = value;
        }

        public int OneupBonus
        {
            get => oneupBonus;
            set => oneupBonus = value;
        }

        public int BreakBlockBonus
        {
            get => breakBlockBonus;
            set => breakBlockBonus = value;
        }

        public bool GamePaused
        {
            get => gamePaused;
            set => gamePaused = value;
        }

        public bool MusicPaused
        {
            get => musicPaused;

            set => musicPaused = value;
        }

        public bool TimerPaused
        {
            get => timerPaused;

            set => timerPaused = value;
        }

        public List<Animator> UnScaledAnimators
        {
            get => base.UnScaledAnimators;
            set => base.UnScaledAnimators = value;
        }

        public float PauseGamePrevTimeScale
        {
            get => base.PauseGamePrevTimeScale;
            set => base.PauseGamePrevTimeScale = value;
        }

        public bool PausePrevMusicPaused
        {
            get => base.PausePrevMusicPaused;
            set => base.PausePrevMusicPaused = value;
        }
        
        public IPauseUnPauseGame GetPauseUnPauseGame => _pauseUnPauseGame;

        #endregion

        private IPauseUnPauseGame _pauseUnPauseGame;

        private void Awake()
        {
            _pauseUnPauseGame = GetComponent<IPauseUnPauseGame>();
        }
    }
}
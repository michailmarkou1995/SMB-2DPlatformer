using System.Collections.Generic;
using Interfaces.UI;
using UnityEngine;

namespace Interfaces.Core.Managers
{
    public interface IGameStateData : IGameStateDataReset
    {
        public int CoinBonus { get; set; }
        public int PowerupBonus { get; set; }
        public int StarmanBonus { get; set; }
        public int OneupBonus { get; set; }
        public int BreakBlockBonus { get; set; }
        public bool GamePaused { get; set; }
        public bool TimerPaused { get; set; }
        public bool MusicPaused { get; set; }
        public List<Animator> UnScaledAnimators { get; set; }
        public float PauseGamePrevTimeScale { get; set; }
        public bool PausePrevMusicPaused { get; set; }
        public IPauseUnPauseGame GetPauseUnPauseGame { get; }
    }
}
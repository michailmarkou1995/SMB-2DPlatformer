using Interfaces.Level;
using Interfaces.UI;

namespace Interfaces.Core.Managers
{
    public interface ILevelManager : ILevelManagerEssentials
    {
        public void RetrieveGameState();
    }

    public interface ILevelManagerEssentials
    {
        // public bool GamePaused { get; set; }
        // public bool TimerPaused { get; set; }
        // public bool MusicPaused { get; set; }
        public bool IsRespawning { get; set; }
        public bool IsPoweringDown { get; set; }
        public ISoundManagerExtras GetSoundManager { get; }
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler { get; }
        public IGameStateManager GetGameStateManager { get; }
        public IHUD GetHUD { get; }
    }
}
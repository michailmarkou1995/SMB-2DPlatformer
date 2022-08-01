using Abilities.Pickups;
using Core.Player;
using Interfaces.Abilities;
using Interfaces.Abilities.Pickups;
using Interfaces.Abilities.Player;
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
        public ISoundManagerExtras GetSoundManager { get; }
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler { get; }
        public IGameStateManager GetGameStateManager { get; }
        public IHUD GetHUD { get; }
        public IPlayerPickUpAbilities GetPlayerPickUpAbilities { get; }
        public IPlayerAbilities GetPlayerAbilities { get; }
        public PlayerController GetPlayerController { get; }
        // public ISoundLevelHandle GetSoundLevelHandle { get; }
        public ILevelServices GetLevelServices { get; }
        public bool TimerPaused { get; set; }
        public bool GamePaused { get; set; }
        public bool MusicPaused { get; set; }
    }
}
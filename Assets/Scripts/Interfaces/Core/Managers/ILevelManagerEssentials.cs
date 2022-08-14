using Interfaces.Abilities.PickUps;
using Interfaces.Abilities.Player;
using Interfaces.Core.Player;
using Interfaces.Level;
using Interfaces.UI;

namespace Interfaces.Core.Managers
{
    public interface ILevelManagerEssentials
    {
        public ISoundManagerExtras GetSoundManager { get; }
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler { get; }
        public IGameStateManager GetGameStateManager { get; }
        public IHUD GetHUD { get; }
        public IPlayerPickUpAbilities GetPlayerPickUpAbilities { get; }
        public IPlayerAbilities GetPlayerAbilities { get; }
        public IPlayerController GetPlayerController { get; }
        public ILevelServices GetLevelServices { get; }
        public IGameStateData GetGameStateData { get; }
        public ISetTimerHUD GetSetTimerHUD { get; }
    }
}
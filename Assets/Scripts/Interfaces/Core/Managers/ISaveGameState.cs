using Core.Managers;

namespace Interfaces.Core.Managers
{
    public interface ISaveGameState
    {
        public void SaveGameState(IGameStateManager gameStateManager = null);
    }
}
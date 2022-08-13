using System;

namespace Interfaces.Core.Managers
{
    public interface IGameStateManager : IGameStateManagerEssentials
    {
        // TODO can split up more per function
        [Obsolete("Not used anymore use base.Awake instead.", true)]
        public void RetainGameStateManagerPerLoad();
        public void ResetSpawnPosition();
        public void SetSpawnPipe(int idx);
        public void ConfigNewGame();
        public void ConfigNewLevel();
        public void ConfigReplayedLevel();
        public void GetSaveGameState();
    }
}
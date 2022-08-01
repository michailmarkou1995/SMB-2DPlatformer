namespace Interfaces.Core.Managers
{
    public interface IGameStateManager : IGameStateManagerEssentials
    {
        // TODO can split up more per function
        public void RetainGameStateManagerPerLoad();
        public void ResetSpawnPosition();
        public void SetSpawnPipe(int idx);
        public void ConfigNewGame();
        public void ConfigNewLevel();
        public void ConfigReplayedLevel();
        public void GetSaveGameState();
        public void PauseUnPauseState();
        public void TimerHUD();
        public void GamePauseCheck();
        public void TimeUpCounter();
    }
}
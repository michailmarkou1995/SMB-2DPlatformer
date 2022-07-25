namespace Core.Managers
{
    public interface IGameStateManager
    {
        public void RetainGameStateManagerPerLoad();
        public void ResetSpawnPosition();
        public void SetSpawnPipe(int idx);
        public void ConfigNewGame();
        public void ConfigNewLevel();
        public void ConfigReplayedLevel();
        public void SaveGameState();
    }
}
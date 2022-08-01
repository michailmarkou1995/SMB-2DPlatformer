namespace Interfaces.Level
{
    public interface ILoadLevel
    {
        void LoadLevel(string loadLevelName = "Main Menu", float delay = 0);
    }

    public interface ILoadLevelSceneHandle : ILoadLevel
    {
        public void LoadSceneDelay(string loadLevelName, float delay = 0);
        public void LoadSceneCurrentLevel(string loadLevelName, float delay = 0);
        public void LoadSceneCurrentLevelSetSpawnPipe(string sceneName, int spawnPipeIdx, float delay = 0);
        public void ReloadCurrentLevel(float delay = 0, bool timeUp = false);
        public void LoadGameOver(float delay = 0, bool timeUp = false);
    }
}
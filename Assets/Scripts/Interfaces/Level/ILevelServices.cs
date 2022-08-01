using UnityEngine;

namespace Interfaces.Level
{
    public interface ILevelServices
    {
        public Vector3 FindSpawnPosition();
        public string GetWorldName(string sceneName);
        public bool IsSceneInCurrentWorld(string sceneName);
        public void MarioCompleteCastle();
        public void MarioCompleteLevel();
        public void MarioReachFlagPole();
    }
}
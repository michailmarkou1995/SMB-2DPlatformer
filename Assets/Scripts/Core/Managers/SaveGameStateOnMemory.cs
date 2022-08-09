#nullable enable
using Interfaces.Core.Managers;
using UnityEngine;

namespace Core.Managers
{
    public class SaveGameStateOnMemory : MonoBehaviour, ISaveGameState
    {
        public void SaveGameState(IGameStateManager gameStateManager)
        {
            //if (gameStateManager == null) throw new NullReferenceException();
            LevelManager? levelManager =
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            if (levelManager == null) return;
            gameStateManager.PlayerSize = levelManager.GetGameStateData.PlayerSize;
            gameStateManager.Lives = levelManager.GetGameStateData.Lives;
            gameStateManager.Coins = levelManager.GetHUD.Coins;
            gameStateManager.Scores = levelManager.GetHUD.Scores;
            gameStateManager.TimeLeft = levelManager.GetGameStateData.TimeLeft;
            gameStateManager.HurryUp = levelManager.GetGameStateData.HurryUp;
        }
    }
}
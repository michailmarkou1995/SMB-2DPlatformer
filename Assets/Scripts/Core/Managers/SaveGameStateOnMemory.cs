using System;
using Interfaces.Core.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Managers
{
    public class SaveGameStateOnMemory : MonoBehaviour, ISaveGameState
    {
        public void SaveGameState(IGameStateManager gameStateManager)
        {
            //if (gameStateManager == null) throw new NullReferenceException();
            LevelManager levelManager = Object.FindObjectOfType<LevelManager>();
            gameStateManager.PlayerSize = levelManager.GetGameStateManager.PlayerSize;
            gameStateManager.Lives = levelManager.GetGameStateManager.Lives;
            gameStateManager.Coins = levelManager.GetHUD.Coins;
            gameStateManager.Scores = levelManager.GetHUD.Scores;
            gameStateManager.TimeLeft = levelManager.GetHUD.TimeLeft;
            gameStateManager.HurryUp = levelManager.GetGameStateManager.HurryUp;
        }
    }
}
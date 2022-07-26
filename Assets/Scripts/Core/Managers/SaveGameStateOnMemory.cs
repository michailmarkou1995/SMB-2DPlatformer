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
            gameStateManager.PlayerSize = levelManager.marioSize;
            gameStateManager.Lives = levelManager.lives;
            gameStateManager.Coins = levelManager.coins;
            gameStateManager.Scores = levelManager.scores;
            gameStateManager.TimeLeft = levelManager.timeLeft;
            gameStateManager.HurryUp = levelManager.hurryUp;
        }
    }
}
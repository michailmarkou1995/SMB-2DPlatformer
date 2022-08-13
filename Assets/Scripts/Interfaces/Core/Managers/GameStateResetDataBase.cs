using Core.Managers;
using Interfaces.Utilities;
using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class GameStateResetDataBase : PersistentSingleton<GameStateManager>
    {
        [SerializeField] protected bool hurryUp; // within last 100 secs?
        [SerializeField] protected int playerSize; // 0..2
        [SerializeField] protected int lives;
        [SerializeField] protected int coins;
        [SerializeField] protected int scores;
        [SerializeField] protected float timeLeft;
    }
}
using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class Death : MonoBehaviour, IDeath
    {
        [field: SerializeField] public bool IsDying { get; set; }
        public float DeadUpTimer { get; set; } = .25f;
    }
}
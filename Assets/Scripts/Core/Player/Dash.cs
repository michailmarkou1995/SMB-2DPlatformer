using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class Dash : MonoBehaviour, IDash
    {
        public bool IsDashing { get; set; }
        public bool WasDashingBeforeJump { get; set; }
    }
}
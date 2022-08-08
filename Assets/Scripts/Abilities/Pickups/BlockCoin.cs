using Core.Managers;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Abilities.Pickups
{
    public class BlockCoin : MonoBehaviour
    {
        private Interfaces.Core.Managers.ILevelManager _levelManager;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            _levelManager.GetPlayerPickUpAbilities.AddCoin(transform.position + Vector3.down);
        }
    }
}
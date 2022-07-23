using Core.Managers;
using UnityEngine;

namespace Abilities.Pickups
{
    public class BlockCoin : MonoBehaviour
    {
        private LevelManager _levelManager;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            _levelManager.AddCoin(transform.position + Vector3.down);
        }
    }
}
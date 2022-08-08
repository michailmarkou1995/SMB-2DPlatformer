using Core.Managers;
using UnityEngine;

namespace Level
{
    public class KillPlane : MonoBehaviour
    {
        private LevelManager _levelManager;

        private void Start()
        {
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player")) {
                _levelManager.GetPlayerAbilities.MarioRespawn();
            } else {
                Destroy(other.gameObject);
            }
        }
    }
}
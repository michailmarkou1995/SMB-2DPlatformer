using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Level
{
    public class PipeWarpSide : MonoBehaviour
    {
        private LevelManager _levelManager;
        private PlayerController _playerController;
        private bool _reachedPortal;

        public string sceneName;
        public int spawnPipeIdx;
        public bool leadToSameLevel = true;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            _playerController = FindObjectOfType<PlayerController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerController.AutomaticWalk(_playerController.LevelEntryWalkSpeedX);
            _reachedPortal = true;
            _levelManager.GetGameStateData.TimerPaused = true;
            Debug.Log(this.name + " OnTriggerEnter2D: " + transform.parent.gameObject.name
                      + " recognizes player, should automatic walk");
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player") || !_reachedPortal) return;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.PipePowerdownSound);

            if (leadToSameLevel) {
                Debug.Log(this.name + " OnCollisionEnter2D: " + transform.parent.gameObject.name
                          + " teleports player to different scene same level " + sceneName
                          + ", pipe idx " + spawnPipeIdx);
                _levelManager.GetLoadLevelSceneHandler.LoadSceneCurrentLevelSetSpawnPipe(sceneName, spawnPipeIdx);
            } else {
                Debug.Log(this.name + " OnCollisionEnter2D: " + transform.parent.gameObject.name
                          + " teleports player to new level " + sceneName
                          + ", pipe idx " + spawnPipeIdx);
                _levelManager.GetLoadLevelSceneHandler.LoadLevel(sceneName);
            }
        }
    }
}
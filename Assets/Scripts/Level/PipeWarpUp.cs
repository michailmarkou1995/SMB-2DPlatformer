using Core.Managers;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
    public class PipeWarpUp : MonoBehaviour
    {
        private IPlayerController _player;
        private Transform _stop;

        private const float PlatformVelocityY = .05f;
        public bool isTakingMarioUp;

        private Interfaces.Core.Managers.ILevelManager _levelManager;

        public bool resetSpawnPoint;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<IPlayerController>();
            _stop = transform.parent.transform.Find("Platform Stop");
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

            Debug.Log(this.name + " Start: " + transform.parent.gameObject.name
                      + " spawnFromPoint=" + _levelManager.GetGameStateManager.SpawnFromPoint.ToString()
                      + " with idx=" + _levelManager.GetGameStateManager.SpawnPipeIdx.ToString());

            if (!_levelManager.GetGameStateManager.SpawnFromPoint &&
                _levelManager.GetGameStateManager.SpawnPipeIdx == transform.parent.GetSiblingIndex()) {
                isTakingMarioUp = true;
                _player.GetMovementFreeze.FreezeUserInput();
                _levelManager.GetGameStateData.TimerPaused = true;
                Debug.Log(this.name + " Start: " + transform.parent.gameObject.name + " taking Mario up");
            } else {
                isTakingMarioUp = false;
                transform.position = _stop.position;
                Debug.Log(this.name + " Start: " + transform.parent.gameObject.name + " not taking Mario up");
            }
        }

        private void FixedUpdate()
        {
            if (!isTakingMarioUp) return;
            if (transform.position.y < _stop.position.y) {
                transform.position = new Vector2(transform.position.x, transform.position.y + PlatformVelocityY);
            } else if (_levelManager.GetGameStateData.TimerPaused) {
                _levelManager.GetGameStateManager.SpawnFromPoint = true;
                if (resetSpawnPoint) {
                    _levelManager.GetGameStateManager.ResetSpawnPosition();
                }

                _player.GetMovementFreeze.UnfreezeUserInput();
                _levelManager.GetGameStateData.TimerPaused = false;
                isTakingMarioUp = false;
            }
        }
    }
}
using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Level
{
    public class PipeWarpUp : MonoBehaviour {
        private PlayerController mario;
        private Transform stop;

        private float platformVelocityY = .05f;
        public bool isTakingMarioUp;

        private LevelManager t_LevelManager;

        public bool resetSpawnPoint = false;

        void Start() {
            mario = FindObjectOfType<PlayerController>();
            stop = transform.parent.transform.Find("Platform Stop");
            GameStateManager _gameStateManager = FindObjectOfType<GameStateManager>();
            t_LevelManager = FindObjectOfType<LevelManager>();

            Debug.Log(this.name + " Start: " + transform.parent.gameObject.name
                      + " spawnFromPoint=" + _gameStateManager.SpawnFromPoint.ToString()
                      + " with idx=" + _gameStateManager.SpawnPipeIdx.ToString());

            if (!_gameStateManager.SpawnFromPoint &&
                _gameStateManager.SpawnPipeIdx == transform.parent.GetSiblingIndex()) {
                isTakingMarioUp = true;
                mario.FreezeUserInput();
                t_LevelManager.timerPaused = true;
                Debug.Log(this.name + " Start: " + transform.parent.gameObject.name + " taking Mario up");
            } else {
                isTakingMarioUp = false;
                transform.position = stop.position;
                Debug.Log(this.name + " Start: " + transform.parent.gameObject.name + " not taking Mario up");
            }
        }

        void FixedUpdate() {
            if (isTakingMarioUp) {
                if (transform.position.y < stop.position.y) {
                    transform.position = new Vector2(transform.position.x, transform.position.y + platformVelocityY);
                } else if (t_LevelManager.timerPaused) {
                    GameStateManager _gameStateManager = FindObjectOfType<GameStateManager>();
                    _gameStateManager.SpawnFromPoint = true;
                    if (resetSpawnPoint) {
                        _gameStateManager.ResetSpawnPosition();
                    }

                    mario.UnfreezeUserInput();
                    t_LevelManager.timerPaused = false;
                    isTakingMarioUp = false;
                }
            }
        }
    }
}
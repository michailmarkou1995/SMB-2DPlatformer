using System;
using Core.Managers;
using Core.Player;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Level
{
    public class PipeWarpUp : MonoBehaviour
    {
        private PlayerController _mario;
        private Transform _stop;

        private const float PlatformVelocityY = .05f;
        public bool isTakingMarioUp;

        private ILevelManager _levelManager;

        public bool resetSpawnPoint;

        private void Start()
        {
            _mario = FindObjectOfType<PlayerController>();
            _stop = transform.parent.transform.Find("Platform Stop");
            IGameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            _levelManager = FindObjectOfType<LevelManager>();

            Debug.Log(this.name + " Start: " + transform.parent.gameObject.name
                      + " spawnFromPoint=" + gameStateManager.SpawnFromPoint.ToString()
                      + " with idx=" + gameStateManager.SpawnPipeIdx.ToString());

            if (!gameStateManager.SpawnFromPoint &&
                gameStateManager.SpawnPipeIdx == transform.parent.GetSiblingIndex()) {
                isTakingMarioUp = true;
                _mario.FreezeUserInput();
                gameStateManager.TimerPaused = true;
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
            } else if (_levelManager.GetGameStateManager.TimerPaused) {
                IGameStateManager _gameStateManager = FindObjectOfType<GameStateManager>();
                _gameStateManager.SpawnFromPoint = true;
                if (resetSpawnPoint) {
                    _gameStateManager.ResetSpawnPosition();
                }

                _mario.UnfreezeUserInput();
                _levelManager.GetGameStateManager.TimerPaused = false;
                isTakingMarioUp = false;
            }
        }
    }
}
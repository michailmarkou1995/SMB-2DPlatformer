using System.Collections;
using Core.Managers;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
    public class PipeWarpSide : MonoBehaviour
    {
        private Interfaces.Core.Managers.ILevelManager _levelManager;
        private IPlayerController _playerController;
        private bool _reachedPortal;

        public string sceneName;
        public int spawnPipeIdx;
        public bool leadToSameLevel = true;

        private void Start()
        {
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<IPlayerController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            StartCoroutine(TouchedGround());
            _playerController.GetMovement.AutomaticWalk(_playerController.GetMovement.LevelEntryWalkSpeedX);
            _reachedPortal = true;
            _levelManager.GetGameStateData.TimerPaused = true;
            Debug.Log(this.name + " OnTriggerEnter2D: " + transform.parent.gameObject.name
                      + " recognizes player, should automatic walk");
        }

        //Fixed Stuck Pipe because AutomaticWalk Kicks in while on "grounded is true"
        private IEnumerator TouchedGround()
        {
            _playerController.GetMovementFreeze.FreezeUserInput();
            yield return new WaitForSeconds(1f);
            _playerController.GetMovementFreeze.UnfreezeUserInput();
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
using System.Collections;
using Interfaces.Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class JumpSteadyFixed : MonoBehaviour, IJump
    {
        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<IPlayerController>();
        }

        public bool IsFalling { get; set; }
        public bool IsJumping { get; set; }
        public bool IsChangedDirOnAirYes { get; set; }
        public bool JumpButtonHeld { get; set; }
        public float AutomaticGravity { get; set; }
        public float NormalGravity { get; set; }
        public float JumpSpeedY { get; set; }
        public float JumpUpGravity { get; set; }
        public float JumpDownGravity { get; set; }

        private void SetJumpParams()
        {
            switch (_playerController.GetMovement.CurrentSpeedX) {
                case < 3.75f:
                    JumpSpeedY = 15f;
                    JumpUpGravity = .47f;
                    JumpDownGravity = 1.64f;
                    break;
                case < 8.67f:
                    JumpSpeedY = 15f;
                    JumpUpGravity = .44f;
                    JumpDownGravity = 1.41f;
                    break;
                default:
                    JumpSpeedY = 18.75f;
                    JumpUpGravity = .59f;
                    JumpDownGravity = 2.11f;
                    break;
            }
        }

        public void Jump_performed(InputAction.CallbackContext context)
        {
            if (_playerController.GetMovementFreeze.InputFreezed) return;
            _playerController.MStompBox.SetActive(true);
            IsJumping = true;
            JumpButtonHeld = true;

            /******** Vertical movement */
            if (_playerController.GetGroundCheck.IsGrounded) {
                _playerController.MRigidbody2D.gravityScale = NormalGravity;
            }

            if (_playerController.GetGroundCheck.IsGrounded && JumpButtonHeld) {
                SetJumpParams();
                _playerController.MRigidbody2D.velocity =
                    new Vector2(_playerController.MRigidbody2D.velocity.x, JumpSpeedY);
                IsJumping = true;
                _playerController.GetMovement.SpeedXBeforeJump = _playerController.GetMovement.CurrentSpeedX;
                _playerController.GetDash.WasDashingBeforeJump = _playerController.GetDash.IsDashing;
                _playerController.GetLevelManager.GetSoundManager.SoundSource.PlayOneShot(
                    _playerController.GetLevelManager.GetGameStateData.PlayerSize == 0
                        ? _playerController.GetLevelManager.GetSoundManager.JumpSmallSound
                        : _playerController.GetLevelManager.GetSoundManager.JumpSuperSound);
            }

            // else if reverse of !isJumping and not released then it is holding it ... long jump!
            if (_playerController.MRigidbody2D.velocity.y > 0)
                _playerController.MRigidbody2D.gravityScale = NormalGravity * JumpUpGravity;
            // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
            StartCoroutine(JumpButtonHeldDelay());
        }

        public void Jump_canceled(InputAction.CallbackContext context)
        {
            // if released then it is not holding it ... short jump! .. default gravity immediately
            if (_playerController.GetMovementFreeze.InputFreezed) return;
            JumpButtonHeld = false;
            _playerController.MRigidbody2D.gravityScale = NormalGravity * JumpDownGravity;
            // Make animator swap to isGrounded and allow physics to process isJumping false conditions(when exit secret pipe)
            StartCoroutine(GroundDelay());
        }

        private IEnumerator GroundDelay()
        {
            // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
            yield return new WaitUntil(() => _playerController.GetGroundCheck.IsGrounded);
            IsJumping = false;
            _playerController.MStompBox.SetActive(false);
        }

        private IEnumerator JumpButtonHeldDelay()
        {
            yield return new WaitForSeconds(.1f);
            StartCoroutine(GroundDelay());
        }

        public void JumpOffPole()
        {
            Transform transformCached = transform;
            Vector3 position = transformCached.position;
            position = new Vector2(position.x + .5f, position.y);
            transformCached.position = position;
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.PoleAnimator, false);
            _playerController.GetMovement.AutomaticWalk(_playerController.GetMovement.CastleWalkSpeedX);
            _playerController.MRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
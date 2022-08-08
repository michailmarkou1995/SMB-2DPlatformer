using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class GroundCheckNonAlloc : MonoBehaviour, IGroundCheck
    {
        //[FormerlySerializedAs("GroundLayers")] public LayerMask groundLayers;
        public int GroundLayers { get; set; }
        public bool IsGrounded { get; set; }

        private IPlayerController _playerController;

        #region Default_GroundLayerMask

        private void Awake()
        {
            _playerController = GetComponentInParent<IPlayerController>();

            // Player Interaction with other Layers e.g., used in raycasts to determine if is jumping or falling.
            GroundLayers = (1 << LayerMask.NameToLayer("Ground")
                            | 1 << LayerMask.NameToLayer("Block")
                            | 1 << LayerMask.NameToLayer("Goal")
                            | 1 << LayerMask.NameToLayer("Player Detector")
                            | 1 << LayerMask.NameToLayer("Moving Platform"));
        }

        #endregion

        public bool IsGround()
        {
            _playerController.Colliders1 = new Collider2D[1];
            _playerController.Colliders2 = new Collider2D[1];
            Physics2D.OverlapPointNonAlloc(_playerController.MGroundCheck1.position,
                _playerController.Colliders1, GroundLayers);
            Physics2D.OverlapPointNonAlloc(_playerController.MGroundCheck2.position,
                _playerController.Colliders2, GroundLayers);
            
            return _playerController.Colliders1[0] || _playerController.Colliders2[0];
            return Physics2D.OverlapPoint(_playerController.MGroundCheck1.position, GroundLayers) ||
                   Physics2D.OverlapPoint(_playerController.MGroundCheck2.position, GroundLayers);
        }
    }
}
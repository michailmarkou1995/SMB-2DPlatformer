using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class GroundCheckAlloc : MonoBehaviour, IGroundCheck
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
            return Physics2D.OverlapPoint(_playerController.MGroundCheck1.position, GroundLayers) ||
                   Physics2D.OverlapPoint(_playerController.MGroundCheck2.position, GroundLayers);
        }
    }
}
namespace Interfaces.Core.Player
{
    public interface IGroundCheck
    {
        public int GroundLayers { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsGround();
    }
}
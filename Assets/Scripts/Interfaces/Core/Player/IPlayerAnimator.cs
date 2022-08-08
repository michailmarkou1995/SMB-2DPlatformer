namespace Interfaces.Core.Player
{
    public interface IPlayerAnimator
    {
        public int PlayerSizeAnimator { get; }
        public int PoleAnimator { get; }
        public int RespawnAnimator { get; }
        public int IsJumpingAnimator { get; }
        public int IsFallingNotFromJumpAnimator { get; }
        public int IsCrouchingAnimator { get; }
        public int AbsSpeedAnimator { get; }
        public int IsFiringAnimator { get; }
        public int IsSkiddingAnimator { get; }
        public int IsInvincibleStarman { get; }
        public int IsInvinciblePowerdown { get; }
        public int IsPoweringUp { get; }
        public int IsPoweringDown { get; }
    }
}
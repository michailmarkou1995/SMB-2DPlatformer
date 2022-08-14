namespace Interfaces.Core.Player
{
    public interface IPlayerAnimationParams
    {
        public void MovementAnimationParams();

        public void IsAnimPowerUp(int animName, bool isPowerUp);
        // public void InvinciblePowerdown_ON();
        // public void InvinciblePowerdown_OFF();
        // public void InvincibleStarman_ON();
        // public void InvincibleStarman_OFF();
    }
}
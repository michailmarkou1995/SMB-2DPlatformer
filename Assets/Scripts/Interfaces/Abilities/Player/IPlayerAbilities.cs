using Core.NPC;
using UnityEngine;

namespace Interfaces.Abilities.Player
{
    public interface IPlayerAbilities
    {
        public bool IsInvincible();
        public void MarioInvincibleStarman();
        public void MarioInvinciblePowerdown();
        public void MarioPowerUp();
        public void MarioPowerDown();
        public void MarioRespawn(bool timeUp = false);
        public void MarioStompEnemy(Enemy enemy);
        public void MarioStarmanTouchEnemy(Enemy enemy);
        public void RollingShellTouchEnemy(Enemy enemy);
        public void BlockHitEnemy(Enemy enemy);
        public void FireballTouchEnemy(Enemy enemy);
        public Rigidbody2D PlayerRigidbody2D { get; set; }
        public bool IsRespawning { get; set; }
        public Vector2 StompBounceVelocity { get; set; }
        public bool IsInvinciblePowerdown { get; set; }
        public bool IsInvincibleStarman { get; set; }
        public bool IsPoweringDown { get; set; }
    }
}
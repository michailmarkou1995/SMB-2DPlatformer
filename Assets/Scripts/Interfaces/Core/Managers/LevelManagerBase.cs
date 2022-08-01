using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.Core.Managers
{
    public abstract class LevelManagerBase : MonoBehaviour
    {
        protected const float LoadSceneDelayTime = 1f;

        public bool hurryUp; // within last 100 secs?
        public int marioSize; // 0..2
        public int lives;
        public int coins;
        public int scores;
        public float timeLeft;
        protected int TimeLeftInt;

        protected bool IsRespawning;
        protected bool IsPoweringDown;

        public bool isInvinciblePowerdown;
        public bool isInvincibleStarman;
        protected const float MarioInvinciblePowerdownDuration = 2;
        protected const float MarioInvincibleStarmanDuration = 12;
        protected const float TransformDuration = 1;

        protected Animator MarioAnimator;
        protected Rigidbody2D MarioRigidbody2D;

        public Text scoreText;
        public Text coinText;
        public Text timeText;
        public GameObject floatingTextEffect;
        protected const float FloatingTextOffsetY = 2f;

        public int coinBonus = 200;
        public int powerupBonus = 1000;
        public int starmanBonus = 1000;
        public int oneupBonus;
        public int breakBlockBonus = 50;

        public Vector2 stompBounceVelocity = new(0, 15);

        public bool gamePaused;
        public bool timerPaused;
        public bool musicPaused;

        protected readonly List<Animator> UnScaledAnimators = new List<Animator>();
        protected float PauseGamePrevTimeScale;
        protected bool PausePrevMusicPaused;
        protected static readonly int IsInvincibleStarmanAnim = Animator.StringToHash("isInvincibleStarman");
        protected static readonly int IsInvinciblePowerdownAnim = Animator.StringToHash("isInvinciblePowerdown");
        protected static readonly int IsPoweringUpAnim = Animator.StringToHash("isPoweringUp");
        protected static readonly int IsPoweringDownAnim = Animator.StringToHash("isPoweringDown");
    }
}
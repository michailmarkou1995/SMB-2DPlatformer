using System.Collections;
using Abilities.NPC;
using Core.Managers;
using Interfaces.Core;
using Interfaces.Core.NPC;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Core.NPC
{
    public class Bowser : BowserBase, IBowser
    {
        #region GettersAndSetters

        public Bowser BowserSelf
        {
            get => this;
        }

        public LevelManager LevelManager
        {
            get => base.LevelManager;
            set => base.LevelManager = value;
        }

        public GameObject Mario
        {
            get => Player;
            set => Player = value;
        }

        public Rigidbody2D Rigidbody2D
        {
            get => GetComponent<Rigidbody2D>();
            set => Rb2D = value;
        }

        public Transform FirePos
        {
            get => firePos;
            set => firePos = value;
        }

        public GameObject BowserImpostor
        {
            get => bowserImpostor;
            set => bowserImpostor = value;
        }

        public GameObject BowserFire
        {
            get => bowserFire;
            set => bowserFire = value;
        }

        public bool CanMove
        {
            get => base.CanMove;
            set => base.CanMove = value;
        }

        public bool Active
        {
            get => base.Active;
            set => base.Active = value;
        }

        public Vector2 ImpostorInitialVelocity
        {
            get => base.ImpostorInitialVelocity;
            set => base.ImpostorInitialVelocity = value;
        }

        public float MinDistanceToMove
        {
            get => base.MinDistanceToMove;
            set => base.MinDistanceToMove = value;
        }

        public int FireResistance
        {
            get => base.FireResistance;
            set => base.FireResistance = value;
        }

        public float WaitBetweenJump
        {
            get => base.WaitBetweenJump;
            set => base.WaitBetweenJump = value;
        }

        public float ShootFireDelay
        {
            get => base.ShootFireDelay;
            set => base.ShootFireDelay = value;
        }

        public float AbsSpeedX
        {
            get => base.AbsSpeedX;
            set => base.AbsSpeedX = value;
        }

        public float DirectionX
        {
            get => base.DirectionX;
            set => base.DirectionX = value;
        }

        public float MinJumpSpeedY
        {
            get => base.MinJumpSpeedY;
            set => base.MinJumpSpeedY = value;
        }

        public float MaxJumpSpeedY
        {
            get => base.MaxJumpSpeedY;
            set => base.MaxJumpSpeedY = value;
        }

        public float Timer
        {
            get => base.Timer;
            set => base.Timer = value;
        }

        public float JumpSpeedY
        {
            get => base.JumpSpeedY;
            set => base.JumpSpeedY = value;
        }

        public int DefeatBonus
        {
            get => base.DefeatBonus;
            set => base.DefeatBonus = value;
        }

        public bool IsFalling
        {
            get => base.IsFalling;
            set => base.IsFalling = value;
        }
        

        #endregion
        private void Start()
        {
            LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            Mario = FindObjectOfType<IPlayerController>().gameObject;
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Timer = 0;
            CanMove = false;
            Active = true;

            starmanBonus = 0;
            rollingShellBonus = 0;
            hitByBlockBonus = 0;
            fireballBonus = 0;
            stompBonus = 0;
            DefeatBonus = 5000;
        }
        
        private void Update()
        {
            BowserSetup();
        }

        public override void TouchedByStarmanMario() { }

        private IEnumerator ShootFireCo(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameObject fire = Instantiate(BowserFire, firePos.position, Quaternion.identity);
            fire.GetComponent<BowserFire>().directionX = transform.localScale.x;
            LevelManager.GetSoundManager.SoundSource.PlayOneShot(LevelManager.GetSoundManager.BowserFireSound);
        }

        public override void TouchedByRollingShell() { }

        public override void HitBelowByBlock() { }

        public override void HitByMarioFireball()
        {
            FireResistance--;
            if (FireResistance > 0) return;
            GameObject impostor = Instantiate(BowserImpostor, transform.position, Quaternion.identity);
            impostor.GetComponent<Rigidbody2D>().velocity =
                new Vector2(ImpostorInitialVelocity.x * DirectionX, ImpostorInitialVelocity.y);
            LevelManager.GetSoundManager.SoundSource.PlayOneShot(LevelManager.GetSoundManager.BowserFallSound);

            LevelManager.GetPlayerPickUpAbilities.AddScore(DefeatBonus);
            Destroy(gameObject);
        }

        public override void StompedByMario() { }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 normal = other.contacts[0].normal;
            Vector2 leftSide = new Vector2(-1f, 0f);
            Vector2 rightSide = new Vector2(1f, 0f);
            bool sideHit = normal == leftSide || normal == rightSide;

            if (!other.gameObject.CompareTag("Player")) return;
            LevelManager.GetPlayerAbilities.MarioPowerDown();
            DirectionX = -DirectionX;
        }

        //From IBowser Interface
        public void ShootFire(float delay = 0)
        {
            StartCoroutine(ShootFireCo(delay));
        }

        public void BowserSetup()
        {
            if (Active) {
                if (!CanMove && Mathf.Abs(Mario.gameObject.transform.position.x - transform.position.x) <=
                    MinDistanceToMove) {
                    CanMove = true;
                }

                if (CanMove) {
                    Rigidbody2D.velocity = new Vector2(DirectionX * AbsSpeedX, Rigidbody2D.velocity.y);
                    Timer -= Time.deltaTime;

                    if (Timer <= 0) {
                        // Turn to face Mario
                        if (Mario.transform.position.x < transform.position.x) {
                            // mario to the left
                            transform.localScale = new Vector3(-1, 1, 1);
                        } else if (Mario.transform.position.x > transform.position.x) {
                            transform.localScale = new Vector3(1, 1, 1);
                        }

                        // Switch walk direction
                        DirectionX = -DirectionX;

                        // Jump a random height
                        JumpSpeedY = Random.Range(MinJumpSpeedY, MaxJumpSpeedY);
                        Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, JumpSpeedY);

                        // Shoot fireball after some delay
                        ShootFire(ShootFireDelay);

                        Timer = WaitBetweenJump;
                    }
                }
            } else if (Rigidbody2D.velocity.y < 0 && !IsFalling) {
                // fall as bridge collapses
                IsFalling = true;
                LevelManager.GetSoundManager.SoundSource.PlayOneShot(LevelManager.GetSoundManager.BowserFallSound);
            }
        }
    }
}
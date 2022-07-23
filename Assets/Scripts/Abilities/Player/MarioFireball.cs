using System.Collections;
using Core.Managers;
using Core.NPC;
using UnityEngine;

namespace Abilities.Player
{
    public class MarioFireball : MonoBehaviour {
        public float directionX; // > 0 for right, < 0 for left
        private const float ExplosionDuration = .25f;
        private readonly Vector2 absVelocity = new Vector2(20, 11);

        private LevelManager tLevelManager;
        private Rigidbody2D mRigidbody2D;
        private Animator mAnimator;
        private static readonly int Exploded = Animator.StringToHash("exploded");

        // Use this for initialization
        private void Awake() {
            // old Start()
            tLevelManager = FindObjectOfType<LevelManager>();
            mRigidbody2D = GetComponent<Rigidbody2D>();
            mAnimator = GetComponent<Animator>();
        }

        private void Start() {
            // initial velocity
            mRigidbody2D.velocity = new Vector2(directionX * absVelocity.x, -absVelocity.y);
        }

        // Update is called once per frame
        private void Update() {
            mRigidbody2D.velocity = new Vector2(directionX * absVelocity.x, mRigidbody2D.velocity.y);
        }

        private void Explode() {
            mRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            mAnimator.SetTrigger(Exploded);
            tLevelManager.soundSource.PlayOneShot(tLevelManager.bumpSound);
            Destroy(gameObject, ExplosionDuration);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Contains("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                tLevelManager.FireballTouchEnemy(enemy);
                Explode();
            } else {
                // bounce off grounds
                Vector2 normal = other.contacts[0].normal;
                Vector2 leftSide = new Vector2(-1f, 0f);
                Vector2 rightSide = new Vector2(1f, 0f);
                Vector2 bottomSide = new Vector2(0f, 1f);

                if (normal == leftSide || normal == rightSide) {
                    // explode if side hit
                    //StartCoroutine(WaitFireBallSpawn());
                    Explode();
                } else if (normal == bottomSide) {
                    // bounce off
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, absVelocity.y);
                } else {
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, -absVelocity.y);
                }
            }
        }

        private IEnumerator WaitFireBallSpawn() {
            yield return new WaitForSeconds(1f);
            Explode();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.tag.Contains("Enemy")) return;
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            tLevelManager.FireballTouchEnemy(enemy);
            Explode();
        }
    }
}
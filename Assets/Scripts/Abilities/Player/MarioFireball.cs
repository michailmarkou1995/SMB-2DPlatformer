using System.Collections;
using Core.Managers;
using Core.NPC;
using UnityEngine;

namespace Abilities.Player
{
    public class MarioFireball : MonoBehaviour {
        public float directionX; // > 0 for right, < 0 for left
        private const float ExplosionDuration = .25f;
        private readonly Vector2 _absVelocity = new Vector2(20, 11);

        private LevelManager _levelManager;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private static readonly int Exploded = Animator.StringToHash("exploded");

        private void Awake() {
            _levelManager = FindObjectOfType<LevelManager>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            // initial velocity
            _rigidbody2D.velocity = new Vector2(directionX * _absVelocity.x, -_absVelocity.y);
        }

        private void Update() {
            _rigidbody2D.velocity = new Vector2(directionX * _absVelocity.x, _rigidbody2D.velocity.y);
        }

        private void Explode() {
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            _animator.SetTrigger(Exploded);
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.BumpSound);
            Destroy(gameObject, ExplosionDuration);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Contains("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                _levelManager.GetPlayerAbilities.FireballTouchEnemy(enemy);
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
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _absVelocity.y);
                } else {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -_absVelocity.y);
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
            _levelManager.GetPlayerAbilities.FireballTouchEnemy(enemy);
            Explode();
        }
    }
}
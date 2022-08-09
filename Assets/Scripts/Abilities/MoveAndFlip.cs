using UnityEngine;
using PlayerController = Core.Player.PlayerController;

/*
 * 
 */

namespace Abilities
{
    /// <summary>
    /// Move continuously, flipping direction if hit on the side by non-Player. Optionally
    /// bounce up if hit ground while moving.
    /// Applicable to: 1UP Mushroom, Big Mushroom, Starman, Goomba, Koopa
    /// </summary>
    public class MoveAndFlip : MonoBehaviour
    {
        public bool canMove;
        public bool canMoveAutomatic = true;
        private const float MinDistanceToMove = 14f;

        public float directionX = 1;
        public Vector2 speed = new(3, 0);
        private Rigidbody2D _rigidbody2D;
        private GameObject _player;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _player = FindObjectOfType<PlayerController>().gameObject;
            OrientSprite();
        }

        private void Update()
        {
            if (!canMove & Mathf.Abs(_player.transform.position.x - transform.position.x) <= MinDistanceToMove &&
                canMoveAutomatic) {
                canMove = true;
            }
        }

        // void OnBecameVisible()
        // {
        //     if (canMoveAutomatic) {
        //         canMove = true;
        //     }
        // }

        // Assuming default sprites face right
        private void OrientSprite()
        {
            switch (directionX) {
                case > 0:
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
                case < 0:
                    transform.localScale = new Vector3(-1, 1, 1);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (canMove) {
                _rigidbody2D.velocity = new Vector2(speed.x * directionX, _rigidbody2D.velocity.y);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 normal = other.contacts[0].normal;
            Vector2 leftSide = new(-1f, 0f);
            Vector2 rightSide = new(1f, 0f);
            Vector2 bottomSide = new(0f, 1f);
            bool sideHit = normal == leftSide || normal == rightSide;
            bool bottomHit = normal == bottomSide;

            // reverse direction
            if (!other.gameObject.CompareTag("Player") && sideHit) {
                directionX = -directionX;
                OrientSprite();
            } else if (other.gameObject.tag.Contains("Platform") && bottomHit && canMove) {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, speed.y);
            }
        }
    }
}
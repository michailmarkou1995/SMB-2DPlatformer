using UnityEngine;

namespace UI
{
    /// <summary>
    ///     Activate sprite renderer/box collider if bumped by Player's head<br />
    ///     Applicable to: Hidden collectible blocks
    /// </summary>
    public class VisibleUponHit : MonoBehaviour
    {
        private BoxCollider2D _boxCollider2D;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _boxCollider2D.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _spriteRenderer.enabled = true;
            _boxCollider2D.enabled = true;
        }
    }
}
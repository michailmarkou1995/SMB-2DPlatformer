using System.Collections;
using UnityEngine;
using PlayerController = Core.Player.PlayerController;

/* Move horizontally and continuously between 2 stop points
 * Applicable to: Horizontal Moving Platform Struct
 */

namespace Abilities.NPC
{
    public class PatrolHorizontal : MonoBehaviour
    {
        public Transform leftStop;
        public Transform rightStop;
        public float absSpeed;
        public float speedModifier = 1;
        public float directionX = 1; // 1 for up, -1 for down
        public bool canMove = false;
        public bool canMoveAutomatic = true; // should object start moving as soon as it's visible?
        private float minDistanceToMove = 150; //14

        public float waitAtLeftStop;
        public float waitAtRightStop;

        public bool isAtLeftStop;
        public bool isAtRightStop;
        private bool _waitLeftCoStarted;
        private bool _waitRightCoStarted;

        private float _currentAbsSpeed;

        private GameObject _player;


        private void Start()
        {
            _player = FindObjectOfType<PlayerController>().gameObject;
            if (transform.position.x >= rightStop.position.x) {
                directionX = -1;
            } else if (transform.position.x <= leftStop.position.x) {
                directionX = 1;
            }

            _currentAbsSpeed = absSpeed;
        }

        private void Update()
        {
            if (!canMove & Mathf.Abs(_player.transform.position.x - transform.position.x) <= minDistanceToMove &&
                canMoveAutomatic) {
                canMove = true;
            } else if (canMove && Time.timeScale != 0) {
                if (!isAtLeftStop && !isAtRightStop) {
                    _currentAbsSpeed *= speedModifier;
                    transform.position += new Vector3(_currentAbsSpeed * directionX, 0, 0);
                    isAtLeftStop = transform.position.x <= leftStop.position.x;
                    isAtRightStop = transform.position.x >= rightStop.position.x;
                } else if (isAtLeftStop && !_waitLeftCoStarted) {
                    StartCoroutine(WaitAtLeftStopCo());
                    _waitLeftCoStarted = true;
                } else if (isAtRightStop && !_waitRightCoStarted) {
                    StartCoroutine(WaitAtRightStopCo());
                    _waitRightCoStarted = true;
                }
            }
        }

        private IEnumerator WaitAtLeftStopCo()
        {
            yield return new WaitForSeconds(waitAtLeftStop);
            _currentAbsSpeed = absSpeed;
            directionX = 1;
            isAtLeftStop = false;
            _waitLeftCoStarted = false;
        }

        private IEnumerator WaitAtRightStopCo()
        {
            yield return new WaitForSeconds(waitAtRightStop);
            _currentAbsSpeed = absSpeed;
            directionX = -1;
            isAtRightStop = false;
            _waitRightCoStarted = false;
        }
    }
}
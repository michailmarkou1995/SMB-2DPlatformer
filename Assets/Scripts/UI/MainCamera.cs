using Core.Managers;
using Core.Player;
using UnityEngine;

namespace UI
{
    public class MainCamera : MonoBehaviour
    {
        public GameObject target;
        public float followAhead = 2.6f;
        public float smoothing = 5;
        public bool canMove;
        public bool canMoveBackward = false;

        private Transform _leftEdge;
        private Transform _rightEdge;
        private float _cameraWidth;
        private Vector3 _targetPosition;


        // Use this for initialization
        private void Start()
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            target = playerController.gameObject;

            GameObject boundary = GameObject.Find("Level Boundary");
            _leftEdge = boundary.transform.Find("Left Boundary").transform;
            _rightEdge = boundary.transform.Find("Right Boundary").transform;
            float aspectRatio = GetComponent<MainCameraAspectRatio>().targetAspects.x /
                                GetComponent<MainCameraAspectRatio>().targetAspects.y;
            if (Camera.main != null) _cameraWidth = Camera.main.orthographicSize * aspectRatio;

            // Initialize camera's position
            Vector3 spawnPosition = FindObjectOfType<LevelManager>().FindSpawnPosition();
            _targetPosition = new Vector3(spawnPosition.x, transform.position.y, transform.position.z);

            bool passedLeftEdge = _targetPosition.x < _leftEdge.position.x + _cameraWidth;

            if (_rightEdge.position.x - _leftEdge.position.x <= _cameraWidth * 2) {
                // center camera if already within boundaries
                transform.position = new Vector3((_leftEdge.position.x + _rightEdge.position.x) / 2f, _targetPosition.y,
                    _targetPosition.z);
                canMove = false;
            } else if (passedLeftEdge) {
                // do not let camera shoot pass left edge
                transform.position = new Vector3(_leftEdge.position.x + _cameraWidth, _targetPosition.y,
                    _targetPosition.z);
                canMove = true;
            } else {
                transform.position = new Vector3(_targetPosition.x + followAhead, _targetPosition.y, _targetPosition.z);
                canMove = true;
            }
        }


        // Update is called once per frame
        // void Update() {
        //     if (canMove) {
        //         bool passedLeftEdge = transform.position.x < leftEdge.position.x + cameraWidth;
        //         bool passedRightEdge = transform.position.x > rightEdge.position.x - cameraWidth;
        //
        //         targetPosition = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
        //
        //         // move target of camera ahead of Player, but do not let camera shoot pass
        //         // level boundaries
        //         if (target.transform.localScale.x > 0f && !passedRightEdge &&
        //             targetPosition.x - leftEdge.position.x >= cameraWidth - followAhead) {
        //             if (canMoveBackward || target.transform.position.x + followAhead >= transform.position.x) {
        //                 targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y, targetPosition.z);
        //                 transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        //             }
        //         } else if (target.transform.localScale.x < 0f && canMoveBackward && !passedLeftEdge
        //                    && rightEdge.position.x - targetPosition.x >= cameraWidth - followAhead) {
        //             targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y, targetPosition.z);
        //             transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        //         }
        //     }
        // }


        private void Update()
        {
            // can move camera both left and right
            if (!canMove) return;
            Vector3 position = transform.position;
            bool passedLeftEdge = position.x < _leftEdge.position.x + _cameraWidth;
            bool passedRightEdge = position.x > _rightEdge.position.x - _cameraWidth;

            _targetPosition = new Vector3(target.transform.position.x, position.y, position.z);

            switch (target.transform.localScale.x) {
                // move target of camera ahead of Player, but do not let camera shoot pass
                // level boundaries
                case > 0f when !passedRightEdge &&
                               _targetPosition.x - _leftEdge.position.x >= _cameraWidth - followAhead:
                    _targetPosition = new Vector3(_targetPosition.x + followAhead, _targetPosition.y,
                        _targetPosition.z);
                    transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothing * Time.deltaTime);
                    break;
                case < 0f when !passedLeftEdge &&
                               _rightEdge.position.x - _targetPosition.x >= _cameraWidth - followAhead:
                    _targetPosition = new Vector3(_targetPosition.x - followAhead, _targetPosition.y,
                        _targetPosition.z);
                    transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothing * Time.deltaTime);
                    break;
            }
        }
    }
}
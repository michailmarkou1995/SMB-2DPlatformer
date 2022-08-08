using Core.Managers;
using Interfaces.UI;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace UI
{
    /// <summary>
    ///     Camera follows target position with going backwards.
    /// </summary>
    public class MainCameraFollowWithBackwards : MonoBehaviour, IMainCameraPosition
    {
        public GameObject target;
        public float followAhead = 2.6f;
        public AnimationCurve followCurve;
        public float smoothing = 5;
        public bool canMove;
        private float _cameraWidth;
        private Camera _camera;

        private Transform _leftEdge;
        private Transform _rightEdge;
        private Vector3 _targetPosition;

        public void GetCameraPrerequisites(MainCamera mainCamera)
        {
            IPlayerController playerController = FindObjectOfType<IPlayerController>();
            target = playerController.gameObject;
            _camera = Camera.main;

            GameObject boundary = GameObject.Find("Level Boundary");
            _leftEdge = boundary.transform.Find("Left Boundary").transform;
            _rightEdge = boundary.transform.Find("Right Boundary").transform;
            float aspectRatio = GetComponent<MainCameraAspectRatio>().targetAspects.x /
                                GetComponent<MainCameraAspectRatio>().targetAspects.y;
            if (_camera != null) _cameraWidth = _camera.orthographicSize * aspectRatio;
        }

        public void InitializeCameraPosition(MainCamera mainCamera)
        {
            Vector3 spawnPosition = FindObjectOfType<LevelManager>().GetLevelServices.FindSpawnPosition();
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

        public void CameraPositionFollows(MainCamera mainCamera)
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
                    _targetPosition = new Vector3(
                        _targetPosition.x + followAhead,
                        _targetPosition.y,
                        _targetPosition.z);
                    transform.position = Vector3.Lerp(
                        transform.position, 
                        _targetPosition, 
                        followCurve.Evaluate(smoothing * Time.deltaTime));
                    break;
                case < 0f when !passedLeftEdge &&
                               _rightEdge.position.x - _targetPosition.x >= _cameraWidth - followAhead:
                    _targetPosition = new Vector3(
                        _targetPosition.x - followAhead, 
                        _targetPosition.y,
                        _targetPosition.z);
                    transform.position = Vector3.Lerp(
                        transform.position, 
                        _targetPosition,
                        followCurve.Evaluate(smoothing * Time.deltaTime));
                    break;
            }
        }
    }
}
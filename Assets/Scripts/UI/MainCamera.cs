using Interfaces.UI;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(IMainCameraPosition))]
    public class MainCamera : MonoBehaviour
    {
        private IMainCameraPosition _followPosition;

        private void Awake()
        {
            _followPosition = GetComponent<IMainCameraPosition>();
        }

        private void Start()
        {
            _followPosition.GetCameraPrerequisites();

            _followPosition.InitializeCameraPosition();
        }

        private void Update()
        {
            _followPosition.CameraPositionFollows();
        }
    }
}
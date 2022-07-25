using UI;

namespace Interfaces.UI
{
    public interface IMainCameraPosition
    {
        public void GetCameraPrerequisites(MainCamera mainCamera = null);
        public void InitializeCameraPosition(MainCamera mainCamera = null);
        public void CameraPositionFollows(MainCamera mainCamera = null);
    }
}
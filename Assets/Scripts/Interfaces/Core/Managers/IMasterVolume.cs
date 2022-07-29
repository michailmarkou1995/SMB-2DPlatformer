using UnityEngine;

namespace Interfaces.Core.Managers
{
    public interface IMasterVolume
    {
        public void SetVolume(GameObject soundSlider = null, GameObject musicSlider = null);
        public void GetSelectVolume(GameObject soundSlider = null, GameObject musicSlider = null);
    }
}
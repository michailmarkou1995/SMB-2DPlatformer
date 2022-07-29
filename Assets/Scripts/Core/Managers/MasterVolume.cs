using Interfaces.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Managers
{
    public class MasterVolume : MonoBehaviour, IMasterVolume
    {
        public void SetVolume(GameObject soundSlider, GameObject musicSlider)
        {
            PlayerPrefs.SetFloat("soundVolume", soundSlider.GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("musicVolume", musicSlider.GetComponent<Slider>().value);
        }

        public void GetSelectVolume(GameObject soundSlider, GameObject musicSlider)
        {
            soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("soundVolume");
            musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");
        }
    }
}
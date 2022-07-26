using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float time;

        [SerializeField] private Text timerText;

        private void Start()
        {
            timerText.text = $"{time:000}";
        }

        private void Update()
        {
            if (!(time > 0)) return;
            time -= Time.deltaTime;
            timerText.text = $"{time:000}";
        }
    }
}
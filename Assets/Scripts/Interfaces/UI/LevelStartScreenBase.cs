using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.UI
{
    public abstract class LevelStartScreenBase : MonoBehaviour
    {
        protected const float LoadScreenDelay = 2f;

        public Text worldTextHUD;
        public Text scoreTextHUD;
        public Text coinTextHUD;
        public Text worldTextMain;
        public Text livesText;
    }
}
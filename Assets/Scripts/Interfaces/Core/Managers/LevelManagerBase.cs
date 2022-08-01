using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.Core.Managers
{
    public abstract class LevelManagerBase : MonoBehaviour
    {
        [SerializeField] protected bool gamePaused;
        [SerializeField] protected bool timerPaused;
        [SerializeField] protected bool musicPaused;
    }
}
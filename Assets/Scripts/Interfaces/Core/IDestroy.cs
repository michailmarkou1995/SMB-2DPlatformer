using UnityEngine;

namespace Interfaces.Core
{
    public interface IDestroy
    {
        public void DestroyGameObj(GameObject gameObject, float delay = 0);
    }
}
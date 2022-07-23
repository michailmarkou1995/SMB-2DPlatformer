using System.IO;
using UI.Interfaces;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Sort Text Effect on Scene
    /// </summary>
    public class FloatingTextEffect : MonoBehaviour, IFloatingTextEffect
    {
        [SerializeField] private string sortingLayerName;

        private void Start()
        {
            MeshRendererSortingLayerName(sortingLayerName);
        }

        public void MeshRendererSortingLayerName(string sortingLayerName)
        {
            if (string.IsNullOrEmpty(sortingLayerName)) {
                Debug.LogError("Sorting layer name is null or empty");
                throw new InvalidDataException("Sorting layer name is null or empty");
                //return;
            }

            GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        }
    }
}
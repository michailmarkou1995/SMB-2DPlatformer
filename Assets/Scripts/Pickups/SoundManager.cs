using UnityEngine;

namespace Pickups {
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        AudioSource audioSource;

        [SerializeField] AudioClip coinSound, pickUpSound;

        private void OnEnable()
        {
            Coin.OnCoinCollected += PlayCoinSound;
        }

        private void PlayCoinSound()
        {
            //Fetch the coin sound
            audioSource.clip = coinSound;
            //Play sound
            audioSource.Play();
        }
    }
}

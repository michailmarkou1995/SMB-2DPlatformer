using UnityEngine;

namespace Interfaces.Core.Managers
{
    public abstract class SoundManagerBase : MonoBehaviour
    {
        [SerializeField] protected AudioSource musicSource;
        [SerializeField] protected AudioSource soundSource;
        [SerializeField] protected AudioSource effectsSource;
        [SerializeField] protected AudioSource pauseSoundSource;

        [SerializeField] protected AudioClip levelMusic;
        [SerializeField] protected AudioClip levelMusicHurry;
        [SerializeField] protected AudioClip starmanMusic;
        [SerializeField] protected AudioClip starmanMusicHurry;
        [SerializeField] protected AudioClip levelCompleteMusic;
        [SerializeField] protected AudioClip castleCompleteMusic;

        [SerializeField] protected AudioClip oneUpSound;
        [SerializeField] protected AudioClip bowserFallSound;
        [SerializeField] protected AudioClip bowserFireSound;
        [SerializeField] protected AudioClip breakBlockSound;
        [SerializeField] protected AudioClip bumpSound;
        [SerializeField] protected AudioClip coinSound;
        [SerializeField] protected AudioClip deadSound;
        [SerializeField] protected AudioClip fireballSound;
        [SerializeField] protected AudioClip flagpoleSound;
        [SerializeField] protected AudioClip jumpSmallSound;
        [SerializeField] protected AudioClip jumpSuperSound;
        [SerializeField] protected AudioClip kickSound;
        [SerializeField] protected AudioClip pipePowerdownSound;
        [SerializeField] protected AudioClip powerupSound;
        [SerializeField] protected AudioClip powerupAppearSound;
        [SerializeField] protected AudioClip stompSound;
        [SerializeField] protected AudioClip warningSound;
    }
}
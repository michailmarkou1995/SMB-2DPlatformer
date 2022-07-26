using Abilities.Pickups;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Core.Managers
{
    #region OldSoundManager

    // public class SoundManager : MonoBehaviour
    // {
    //     public static SoundManager Instance;
    //     private AudioSource _audioSource;
    //
    //     [SerializeField] private AudioClip coinSound, pickUpSound;
    //
    //     private void OnEnable()
    //     {
    //         Coin.OnCoinCollected += PlayCoinSound;
    //     }
    //
    //     private void PlayCoinSound()
    //     {
    //         //Fetch the coin sound
    //         _audioSource.clip = coinSound;
    //         //Play sound
    //         _audioSource.Play();
    //     }
    // }

    #endregion

    public class SoundManager : MonoBehaviour, ISoundManagerExtras
    {
        public AudioSource musicSource;
        public AudioSource soundSource;
        public AudioSource pauseSoundSource;

        public AudioClip levelMusic;
        public AudioClip levelMusicHurry;
        public AudioClip starmanMusic;
        public AudioClip starmanMusicHurry;
        public AudioClip levelCompleteMusic;
        public AudioClip castleCompleteMusic;

        public AudioClip oneUpSound;
        public AudioClip bowserFallSound;
        public AudioClip bowserFireSound;
        public AudioClip breakBlockSound;
        public AudioClip bumpSound;
        public AudioClip coinSound;
        public AudioClip deadSound;
        public AudioClip fireballSound;
        public AudioClip flagpoleSound;
        public AudioClip jumpSmallSound;
        public AudioClip jumpSuperSound;
        public AudioClip kickSound;
        public AudioClip pipePowerdownSound;
        public AudioClip powerupSound;
        public AudioClip powerupAppearSound;
        public AudioClip stompSound;
        public AudioClip warningSound;

        public AudioSource MusicSource
        {
            get => musicSource;
            set => musicSource = value;
        }

        public AudioSource SoundSource
        {
            get => soundSource;
            set => soundSource = value;
        }

        public AudioSource PauseSoundSource
        {
            get => pauseSoundSource;
            set => pauseSoundSource = value;
        }

        public AudioClip LevelMusic
        {
            get => levelMusic;
            set => levelMusic = value;
        }

        public AudioClip LevelMusicHurry
        {
            get => levelMusicHurry;
            set => levelMusicHurry = value;
        }

        public AudioClip StarmanMusic
        {
            get => starmanMusic;
            set => starmanMusic = value;
        }

        public AudioClip StarmanMusicHurry
        {
            get => starmanMusicHurry;
            set => starmanMusicHurry = value;
        }

        public AudioClip LevelCompleteMusic
        {
            get => levelCompleteMusic;
            set => levelCompleteMusic = value;
        }

        public AudioClip CastleCompleteMusic
        {
            get => castleCompleteMusic;
            set => castleCompleteMusic = value;
        }

        public AudioClip OneUpSound
        {
            get => oneUpSound;
            set => oneUpSound = value;
        }

        public AudioClip BowserFallSound
        {
            get => bowserFallSound;
            set => bowserFallSound = value;
        }

        public AudioClip BowserFireSound
        {
            get => bowserFireSound;
            set => bowserFireSound = value;
        }

        public AudioClip BreakBlockSound
        {
            get => breakBlockSound;
            set => breakBlockSound = value;
        }

        public AudioClip BumpSound
        {
            get => bumpSound;
            set => bumpSound = value;
        }

        public AudioClip CoinSound
        {
            get => coinSound;
            set => coinSound = value;
        }

        public AudioClip DeadSound
        {
            get => deadSound;
            set => deadSound = value;
        }

        public AudioClip FireballSound
        {
            get => fireballSound;
            set => fireballSound = value;
        }

        public AudioClip FlagpoleSound
        {
            get => flagpoleSound;
            set => flagpoleSound = value;
        }

        public AudioClip JumpSmallSound
        {
            get => jumpSmallSound;
            set => jumpSmallSound = value;
        }

        public AudioClip JumpSuperSound
        {
            get => jumpSuperSound;
            set => jumpSuperSound = value;
        }

        public AudioClip KickSound
        {
            get => kickSound;
            set => kickSound = value;
        }

        public AudioClip PipePowerdownSound
        {
            get => pipePowerdownSound;
            set => pipePowerdownSound = value;
        }

        public AudioClip PowerupSound
        {
            get => powerupSound;
            set => powerupSound = value;
        }

        public AudioClip PowerupAppearSound
        {
            get => powerupAppearSound;
            set => powerupAppearSound = value;
        }

        public AudioClip StompSound
        {
            get => stompSound;
            set => stompSound = value;
        }

        public AudioClip WarningSound
        {
            get => warningSound;
            set => warningSound = value;
        }
    }
}
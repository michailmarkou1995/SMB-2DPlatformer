using Interfaces.Core.Managers;
using Interfaces.Level;
using JetBrains.Annotations;
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

    // [RequireComponent(typeof(ISoundManagerExtras))]
    // [RequireComponent(typeof(ISoundLevelHandle))]
    public class SoundManager : SoundManagerBase, ISoundManagerExtras
    {
        // public static ISoundManager Instance { get; private set; }

        // private void Awake()
        // {
        //     if (Instance == null)
        //     {
        //         Instance = this;
        //         DontDestroyOnLoad(gameObject);
        //     }
        //     else
        //     {
        //         Destroy(gameObject);
        //     }
        // }

        [CanBeNull] private IMasterVolume _masterVolume;
        private ISoundLevelHandle _soundLevelHandler;

        public ISoundLevelHandle GetSoundLevelHandle => _soundLevelHandler;

        private void Awake()
        {
            _soundLevelHandler = GetComponent<ISoundLevelHandle>();
        }

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

        public AudioSource EffectsSource
        {
            get => effectsSource;
            set => effectsSource = value;
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

        public void GetSoundVolume()
        {
            MusicSource.volume = PlayerPrefs.GetFloat("musicVolume");
            SoundSource.volume = PlayerPrefs.GetFloat("soundVolume");
            PauseSoundSource.volume = PlayerPrefs.GetFloat("soundVolume");
        }

        public void PlaySound(AudioClip clip)
        {
            // SoundSource.clip = clip;
            // SoundSource.Play();
            effectsSource.PlayOneShot(clip);
        }
    }
}
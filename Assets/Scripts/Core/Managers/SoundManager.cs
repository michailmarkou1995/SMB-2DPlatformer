using System;
using System.Collections;
using Interfaces.Core.Managers;
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

    public class SoundManager : SoundManagerBase, ISoundManagerExtras
    {
        // public static ISoundManager Instance { get; private set; }

        [CanBeNull] private IMasterVolume _masterVolume;
        [CanBeNull] private ISoundHandle _soundHandle;

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

    internal interface ISoundHandle
    {
        public void ChangeMusic(AudioClip clip, float delay = 0);
        public void PauseMusicPlaySound(AudioClip clip, bool resumeMusic);
    }

    public class LevelHandleMusic : MonoBehaviour, ISoundHandle
    {
        public bool gamePaused;
        public bool timerPaused;
        public bool musicPaused;
        
        private ISoundManagerExtras _soundManager;
        
        public void ChangeMusic(AudioClip clip, float delay = 0)
        {
            StartCoroutine(ChangeMusicCo(clip, delay));
        }
        
        private IEnumerator ChangeMusicCo(AudioClip clip, float delay)
        {
            Debug.Log(this.name + " ChangeMusicCo: starts changing music to " + clip.name);
            _soundManager.MusicSource.clip = clip;
            yield return new WaitWhile(() => gamePaused);
            yield return new WaitForSecondsRealtime(delay);
            yield return new WaitWhile(() => gamePaused || musicPaused);
            // if (!IsRespawning) {
            //     _soundManager.MusicSource.Play();
            // }

            Debug.Log(this.name + " ChangeMusicCo: done changing music to " + clip.name);
        }

        public void PauseMusicPlaySound(AudioClip clip, bool resumeMusic)
        {
            StartCoroutine(PauseMusicPlaySoundCo(clip, resumeMusic));
        }
        
        private IEnumerator PauseMusicPlaySoundCo(AudioClip clip, bool resumeMusic)
        {
            string musicClipName = "";
            if (_soundManager.MusicSource.clip) {
                musicClipName = _soundManager.MusicSource.clip.name;
            }

            Debug.Log(this.name + " Pause musicPlaySoundCo: starts pausing music " + musicClipName + " to play sound " +
                      clip.name);

            musicPaused = true;
            _soundManager.MusicSource.Pause();
            _soundManager.SoundSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
            if (resumeMusic) {
                _soundManager.MusicSource.UnPause();

                musicClipName = "";
                if (_soundManager.MusicSource.clip) {
                    musicClipName = _soundManager.MusicSource.clip.name;
                }

                Debug.Log(this.name + " PausemusicPlaySoundCo: resume playing music " + musicClipName);
            }

            musicPaused = false;

            Debug.Log(this.name + " PausemusicPlaySoundCo: done pausing music to play sound " + clip.name);
        }
    }
}
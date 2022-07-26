using UnityEngine;

namespace Interfaces.Core.Managers
{
    public interface ISoundManager
    {
        public AudioSource MusicSource { get; set; }
        public AudioSource SoundSource { get; set; }
        public AudioSource PauseSoundSource { get; set; }
        public AudioClip LevelMusic { get; set; }
        public AudioClip LevelMusicHurry { get; set; }
        public AudioClip LevelCompleteMusic { get; set; }
        public AudioClip OneUpSound { get; set; }
        public AudioClip BreakBlockSound { get; set; }
        public AudioClip BumpSound { get; set; }
        public AudioClip CoinSound { get; set; }
        public AudioClip DeadSound { get; set; }
        public AudioClip JumpSmallSound { get; set; }
        public AudioClip JumpSuperSound { get; set; }
        public AudioClip KickSound { get; set; }
        public AudioClip PowerupSound { get; set; }
        public AudioClip PowerupAppearSound { get; set; }
        public AudioClip WarningSound { get; set; }
    }
}
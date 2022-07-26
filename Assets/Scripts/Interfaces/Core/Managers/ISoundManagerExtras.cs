using UnityEngine;

namespace Interfaces.Core.Managers
{
    public interface ISoundManagerExtras : ISoundManager
    {
        public AudioClip StarmanMusic { get; set; }
        public AudioClip StarmanMusicHurry { get; set; }
        public AudioClip CastleCompleteMusic { get; set; }
        public AudioClip BowserFallSound { get; set; }
        public AudioClip BowserFireSound { get; set; }
        public AudioClip FireballSound { get; set; }
        public AudioClip FlagpoleSound { get; set; }
        public AudioClip PipePowerdownSound { get; set; }
        public AudioClip StompSound { get; set; }
    }
}
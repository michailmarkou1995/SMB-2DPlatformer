using UnityEngine;

namespace Interfaces.Level
{
    internal interface ISoundLevelHandle
    {
        public void ChangeMusic(AudioClip clip, float delay = 0);
        public void PauseMusicPlaySound(AudioClip clip, bool resumeMusic);
    }
}
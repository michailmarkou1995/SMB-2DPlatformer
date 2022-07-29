using System.Collections;
using Core.Managers;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;

namespace Level
{
    public class LevelHandleMusic : MonoBehaviour, ISoundLevelHandle
    {
        private ILevelManager _levelManager;
        private ISoundManagerExtras _soundManager;

        private void Awake()
        {
            _levelManager = GetComponent<LevelManager>();
            _soundManager = GetComponent<SoundManager>();
        }

        public void ChangeMusic(AudioClip clip, float delay = 0)
        {
            StartCoroutine(ChangeMusicCo(clip, delay));
        }

        private IEnumerator ChangeMusicCo(AudioClip clip, float delay)
        {
            Debug.Log(this.name + " ChangeMusicCo: starts changing music to " + clip.name);
            _soundManager.MusicSource.clip = clip;
            yield return new WaitWhile(() => _levelManager.GetGameStateManager.GamePaused);
            yield return new WaitForSecondsRealtime(delay);
            yield return new WaitWhile(() =>
                _levelManager.GetGameStateManager.GamePaused || _levelManager.GetGameStateManager.MusicPaused);
            if (!_levelManager.IsRespawning) {
                _soundManager.MusicSource.Play();
            }

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

            _levelManager.GetGameStateManager.MusicPaused = true;
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

            _levelManager.GetGameStateManager.MusicPaused = false;

            Debug.Log(this.name + " PausemusicPlaySoundCo: done pausing music to play sound " + clip.name);
        }
    }
}
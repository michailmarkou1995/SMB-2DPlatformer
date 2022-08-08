using System.Collections;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Core.Managers
{
    public class PauseUnPauseGame : MonoBehaviour, IPauseUnPauseGame
    {
        private Interfaces.Core.Managers.ILevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<LevelManager>();
        }

        public void PauseUnPauseState()
        {
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            StartCoroutine(!_levelManager.GetGameStateData.GamePaused ? PauseGameCo() : UnpauseGameCo());
        }

        private IEnumerator PauseGameCo()
        {
            _levelManager.GetGameStateData.GamePaused = true;
            _levelManager.GetGameStateData.PauseGamePrevTimeScale = Time.timeScale;

            Time.timeScale = 0;
            _levelManager.GetGameStateData.PausePrevMusicPaused = _levelManager.GetGameStateData.MusicPaused;
            _levelManager.GetSoundManager.MusicSource.Pause();
            _levelManager.GetGameStateData.MusicPaused = true;
            _levelManager.GetSoundManager.SoundSource.Pause();

            // Set any active animators that use unscaled time mode to normal
            _levelManager.GetGameStateData.UnScaledAnimators.Clear();
            foreach (Animator animator in FindObjectsOfType<Animator>()) {
                if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
                _levelManager.GetGameStateData.UnScaledAnimators.Add(animator);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            _levelManager.GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(_levelManager.GetSoundManager.PauseSoundSource.clip.length);
            Debug.Log(this.name + " PauseGameCo stops: records prevTimeScale=" + _levelManager.GetGameStateData.PauseGamePrevTimeScale.ToString());
        }

        private IEnumerator UnpauseGameCo()
        {
            _levelManager.GetSoundManager.PauseSoundSource.Play();
            yield return new WaitForSecondsRealtime(_levelManager.GetSoundManager.PauseSoundSource.clip.length);

            _levelManager.GetGameStateData.MusicPaused = _levelManager.GetGameStateData.PausePrevMusicPaused;
            if (!_levelManager.GetGameStateData.MusicPaused) {
                _levelManager.GetSoundManager.MusicSource.UnPause();
            }

            _levelManager.GetSoundManager.SoundSource.UnPause();

            // Reset animators
            foreach (Animator animator in _levelManager.GetGameStateData.UnScaledAnimators) {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            _levelManager.GetGameStateData.UnScaledAnimators.Clear();

            Time.timeScale = _levelManager.GetGameStateData.PauseGamePrevTimeScale;
            _levelManager.GetGameStateData.GamePaused = false;
            Debug.Log(this.name + " UnpauseGameCo stops: resume prevTimeScale=" + _levelManager.GetGameStateData.PauseGamePrevTimeScale.ToString());
        }
        
        public void GamePauseCheck()
        {
            if (!Input.GetButtonDown("Pause")) return;
            PauseUnPauseState();
        }
    }
}
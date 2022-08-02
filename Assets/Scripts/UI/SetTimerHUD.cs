using Interfaces.Core.Managers;
using Interfaces.UI;
using UnityEngine;

namespace UI
{
    public class SetTimerHUD : MonoBehaviour, ISetTimerHUD
    {
        private ILevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public void TimerHUD()
        {
            if (_levelManager.GetGameStateData.TimerPaused) return;
            _levelManager.GetGameStateData.TimeLeft -= Time.deltaTime; // / .4f; // 1 game sec ~ 0.4 real time sec
            _levelManager.GetHUD.SetHudTime();
        }

        public void TimeUpCounter()
        {
            if (_levelManager.GetHUD.TimeLeftIntHUD <= 0) {
                _levelManager.GetPlayerAbilities.MarioRespawn(true);
            }
        }

        public void TimerHUDMusic()
        {
            if (_levelManager.GetHUD.TimeLeftIntHUD >= 100 || _levelManager.GetGameStateManager.HurryUp) return;
            _levelManager.GetGameStateManager.HurryUp = true;
            _levelManager.GetSoundManager.GetSoundLevelHandle.PauseMusicPlaySound(
                _levelManager.GetSoundManager.WarningSound, true);
            _levelManager.GetSoundManager.GetSoundLevelHandle.ChangeMusic(
                _levelManager.GetPlayerAbilities.IsInvincibleStarman
                    ? _levelManager.GetSoundManager.StarmanMusicHurry
                    : _levelManager.GetSoundManager.LevelMusicHurry,
                _levelManager.GetSoundManager.WarningSound.length);
        }
    }
}
using Interfaces.Abilities.PickUps;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Abilities.Pickups
{
    public class PlayerPickUpAbilities : MonoBehaviour, IPlayerPickUpAbilities
    {
        private ILevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public void AddLife()
        {
            _levelManager.GetGameStateData.Lives++;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.OneUpSound);
        }

        public void AddLife(Vector3 spawnPos)
        {
            _levelManager.GetGameStateData.Lives++;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.OneUpSound);
            _levelManager.GetHUD.CreateFloatingText("1UP", spawnPos);
        }

        public void AddCoin()
        {
            _levelManager.GetHUD.Coins++;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.CoinSound);
            if (_levelManager.GetHUD.Coins == 100) {
                AddLife();
                _levelManager.GetHUD.Coins = 0;
            }

            _levelManager.GetHUD.SetHudCoin();
            AddScore(_levelManager.GetGameStateData.CoinBonus);
        }

        public void AddCoin(Vector3 spawnPos)
        {
            _levelManager.GetHUD.Coins++;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.CoinSound);
            if (_levelManager.GetHUD.Coins == 100) {
                AddLife();
                _levelManager.GetHUD.Coins = 0;
            }

            _levelManager.GetHUD.SetHudCoin();
            AddScore(_levelManager.GetGameStateData.CoinBonus, spawnPos);
        }

        public void AddScore(int bonus)
        {
            _levelManager.GetHUD.Scores += bonus;
            _levelManager.GetHUD.SetHudScore();
        }

        public void AddScore(int bonus, Vector3 spawnPos)
        {
            _levelManager.GetHUD.Scores += bonus;
            _levelManager.GetHUD.SetHudScore();
            if (bonus > 0) {
                _levelManager.GetHUD.CreateFloatingText(bonus.ToString(), spawnPos);
            }
        }
    }
    // public class PlayerPickUpAbilities : MonoBehaviour, IPlayerPickUpAbilities
    // {
    //     public int coinBonus = 200;
    //
    //     private ILevelManager _levelManager;
    //
    //     public void AddLife(Vector3 spawnPos = default)
    //     {
    //         if (spawnPos == default) {
    //             _levelManager.GetGameStateManager.Lives++;
    //             _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.OneUpSound);
    //         } else {
    //             _levelManager.GetGameStateManager.Lives++;
    //             _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.OneUpSound);
    //             _levelManager.GetHUD.CreateFloatingText("1UP", spawnPos);
    //         }
    //     }
    //
    //     public void AddCoin(Vector3 spawnPos = default)
    //     {
    //         if (spawnPos == default) {
    //             _levelManager.GetHUD.Coins++;
    //             _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.CoinSound);
    //             if (_levelManager.GetHUD.Coins == 100) {
    //                 AddLife();
    //                 _levelManager.GetHUD.Coins = 0;
    //             }
    //
    //             _levelManager.GetHUD.SetHudCoin();
    //             AddScore(coinBonus);
    //         } else {
    //             _levelManager.GetHUD.Coins++;
    //             _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.CoinSound);
    //             if (_levelManager.GetHUD.Coins == 100) {
    //                 AddLife();
    //                 _levelManager.GetHUD.Coins = 0;
    //             }
    //
    //             _levelManager.GetHUD.SetHudCoin();
    //             AddScore(coinBonus, spawnPos);
    //         }
    //     }
    //
    //     public void AddScore(int? bonus = null, Vector3 spawnPos = default)
    //     {
    //         if (bonus == null) {
    //             _levelManager.GetHUD.Scores += bonus ?? default(int);
    //             _levelManager.GetHUD.SetHudScore();
    //         } else {
    //             _levelManager.GetHUD.Scores += bonus ?? default(int);
    //             _levelManager.GetHUD.SetHudScore();
    //             if (bonus > 0) {
    //                 _levelManager.GetHUD.CreateFloatingText(bonus.ToString(), spawnPos);
    //             }
    //         }
    //     }
    // }
}
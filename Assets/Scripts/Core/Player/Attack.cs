using System;
using Abilities.Player;
using Interfaces.Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class Attack : MonoBehaviour, IAttack
    {
        private const float WaitBetweenFire = .2f;

        [FormerlySerializedAs("Fireball")] public GameObject fireball;
        [FormerlySerializedAs("FirePos")] public Transform firePos;

        public float FireTime1 { get; set; }
        public float FireTime2 { get; set; }
        public bool IsShooting { get; set; }

        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            if (firePos == null) throw new NullReferenceException();
            if (fireball == null) throw new NullReferenceException();
        }

        public void Shooting(InputAction.CallbackContext obj)
        {
            if (!IsShooting || _playerController.GetLevelManager.GetGameStateData.PlayerSize != 2) return;
            FireTime2 = Time.time;
            
            if (!(FireTime2 - FireTime1 >= WaitBetweenFire)) return;
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetTrigger(PlayerAnimatorStatic.IsFiringAnimator);
            GameObject fireball = Instantiate(this.fireball, firePos.position, Quaternion.identity);
            fireball.GetComponent<MarioFireball>().directionX = transform.localScale.x;
            _playerController.GetLevelManager.GetSoundManager.SoundSource.PlayOneShot(_playerController.GetLevelManager
                .GetSoundManager.FireballSound);
            FireTime1 = Time.time;
        }
    }
}
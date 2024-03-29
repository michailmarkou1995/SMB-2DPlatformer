﻿using System.Collections;
using Core.NPC;
using Core.Player;
using Interfaces.Abilities.Player;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Abilities.Player
{
    public class PlayerAbilities : PlayerAbilitiesBase, IPlayerAbilities
    {
        private ILevelManager _levelManager;

        public Rigidbody2D PlayerRigidbody2D
        {
            get => _marioRigidbody2D;
            set => _marioRigidbody2D = value;
        }

        public bool IsRespawning
        {
            get => _isRespawning;
            set => _isRespawning = value;
        }

        public Vector2 StompBounceVelocity
        {
            get => stompBounceVelocity;
            set => stompBounceVelocity = value;
        }

        public bool IsPoweringDown
        {
            get => isPoweringDown;
            set => isPoweringDown = value;
        }

        public bool IsInvinciblePowerdown
        {
            get => isInvinciblePowerdown;
            set => isInvinciblePowerdown = value;
        }

        public bool IsInvincibleStarman
        {
            get => isInvincibleStarman;
            set => isInvincibleStarman = value;
        }

        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public bool IsInvincible()
        {
            return IsInvinciblePowerdown || IsInvincibleStarman;
        }

        public void MarioInvincibleStarman()
        {
            StartCoroutine(MarioInvincibleStarmanCo());
            _levelManager.GetPlayerPickUpAbilities.AddScore(_levelManager.GetGameStateData.StarmanBonus,
                _levelManager.GetPlayerController.transform.position);
        }

        private IEnumerator MarioInvincibleStarmanCo()
        {
            IsInvincibleStarman = true;
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsInvincibleStarmanAnim, true);
            _levelManager.GetPlayerController.gameObject.layer = LayerMask.NameToLayer("Mario After Starman");
            _levelManager.GetSoundManager.GetSoundLevelHandle.ChangeMusic(_levelManager.GetGameStateData.HurryUp
                ? _levelManager.GetSoundManager.StarmanMusicHurry
                : _levelManager.GetSoundManager.StarmanMusic);

            yield return new WaitForSeconds(MarioInvincibleStarmanDuration);
            IsInvincibleStarman = false;
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsInvincibleStarmanAnim, false);
            _levelManager.GetPlayerController.gameObject.layer = LayerMask.NameToLayer("Mario");
            _levelManager.GetSoundManager.GetSoundLevelHandle.ChangeMusic(_levelManager.GetGameStateData.HurryUp
                ? _levelManager.GetSoundManager.LevelMusicHurry
                : _levelManager.GetSoundManager.LevelMusic);
        }

        public void MarioInvinciblePowerdown()
        {
            StartCoroutine(MarioInvinciblePowerdownCo());
        }

        private IEnumerator MarioInvinciblePowerdownCo()
        {
            IsInvinciblePowerdown = true;

            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsInvinciblePowerdownAnim, true);
            _levelManager.GetPlayerController.gameObject.layer = LayerMask.NameToLayer("Mario After Powerdown");
            yield return new WaitForSeconds(MarioInvinciblePowerdownDuration);
            IsInvinciblePowerdown = false;
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsInvinciblePowerdownAnim, false);
            _levelManager.GetPlayerController.gameObject.layer = LayerMask.NameToLayer("Mario");
        }

        public void MarioPowerUp()
        {
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager
                .PowerupSound); // should play sound regardless of size
            if (_levelManager.GetGameStateData.PlayerSize < 2) {
                StartCoroutine(MarioPowerUpCo());
            }

            _levelManager.GetPlayerPickUpAbilities.AddScore(_levelManager.GetGameStateData.PowerupBonus,
                _levelManager.GetPlayerController.transform.position);
        }

        private IEnumerator MarioPowerUpCo()
        {
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsPoweringUpAnim, true);
            Time.timeScale = 0f;
            PlayerAnimatorStatic.PlayerAnimatorComponent.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => _levelManager.GetGameStateData.GamePaused);

            Time.timeScale = 1;
            PlayerAnimatorStatic.PlayerAnimatorComponent.updateMode = AnimatorUpdateMode.Normal;

            _levelManager.GetGameStateData.PlayerSize++;
            _levelManager.GetPlayerController.GetPlayerSize.UpdateSize();
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsPoweringUpAnim, false);        }

        public void MarioPowerDown()
        {
            if (!IsPoweringDown) {
                Debug.Log(this.name + " MarioPowerDown: called and executed");
                IsPoweringDown = true;

                if (_levelManager.GetGameStateData.PlayerSize > 0) {
                    StartCoroutine(MarioPowerDownCo());
                    _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager
                        .PipePowerdownSound);
                } else {
                    MarioRespawn();
                }

                Debug.Log(this.name + " MarioPowerDown: done executing");
            } else {
                Debug.Log(this.name + " MarioPowerDown: called but not executed");
            }
        }

        private IEnumerator MarioPowerDownCo()
        {
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsPoweringDownAnim, true);
            Time.timeScale = 0f;
            PlayerAnimatorStatic.PlayerAnimatorComponent.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitForSecondsRealtime(TransformDuration);
            yield return new WaitWhile(() => _levelManager.GetGameStateData.GamePaused);

            Time.timeScale = 1;
            PlayerAnimatorStatic.PlayerAnimatorComponent.updateMode = AnimatorUpdateMode.Normal;
            MarioInvinciblePowerdown();

            _levelManager.GetGameStateData.PlayerSize = 0;
            _levelManager.GetPlayerController.GetPlayerSize.UpdateSize();
            _levelManager.GetPlayerController.GetAnimationParams.IsAnimPowerUp(
                PlayerAnimatorStatic.IsPoweringDownAnim, false);
            IsPoweringDown = false;
        }

        public void MarioRespawn(bool timeUp = false)
        {
            if (_levelManager.GetPlayerAbilities.IsRespawning) return;
            _levelManager.GetPlayerAbilities.IsRespawning = true;

            _levelManager.GetGameStateData.PlayerSize = 0;
            _levelManager.GetGameStateData.Lives--;

            _levelManager.GetSoundManager.SoundSource.Stop();
            _levelManager.GetSoundManager.MusicSource.Stop();
            _levelManager.GetGameStateData.MusicPaused = true;
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.DeadSound);

            Time.timeScale = 0f;
            _levelManager.GetPlayerController.GetMovementFreeze.FreezeAndDie();

            if (timeUp) {
                Debug.Log(this.name + " MarioRespawn: called due to timeup");
            }

            Debug.Log(this.name + " MarioRespawn: lives left=" + _levelManager.GetGameStateData.Lives.ToString());

            if (_levelManager.GetGameStateData.Lives > 0) {
                _levelManager.GetLoadLevelSceneHandler.ReloadCurrentLevel(
                    _levelManager.GetSoundManager.DeadSound.length, timeUp);
            } else {
                _levelManager.GetLoadLevelSceneHandler.LoadGameOver(_levelManager.GetSoundManager.DeadSound.length,
                    timeUp);
                Debug.Log(this.name + " MarioRespawn: all dead");
            }
        }

        /****************** Kill enemy */
        public void MarioStompEnemy(Enemy enemy)
        {
            _marioRigidbody2D.velocity =
                new Vector2(_marioRigidbody2D.velocity.x + stompBounceVelocity.x, stompBounceVelocity.y);
            enemy.StompedByMario();
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.StompSound);
            _levelManager.GetPlayerPickUpAbilities.AddScore(enemy.stompBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStompEnemy called on " + enemy.gameObject.name);
        }

        public void MarioStarmanTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByStarmanMario();
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.KickSound);
            _levelManager.GetPlayerPickUpAbilities.AddScore(enemy.starmanBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " MarioStarmanTouchEnemy called on " + enemy.gameObject.name);
        }

        public void RollingShellTouchEnemy(Enemy enemy)
        {
            enemy.TouchedByRollingShell();
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.KickSound);
            _levelManager.GetPlayerPickUpAbilities.AddScore(enemy.rollingShellBonus,
                enemy.gameObject.transform.position);
            Debug.Log(this.name + " RollingShellTouchEnemy called on " + enemy.gameObject.name);
        }

        public void BlockHitEnemy(Enemy enemy)
        {
            enemy.HitBelowByBlock();
            _levelManager.GetPlayerPickUpAbilities.AddScore(enemy.hitByBlockBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " BlockHitEnemy called on " + enemy.gameObject.name);
        }

        public void FireballTouchEnemy(Enemy enemy)
        {
            enemy.HitByMarioFireball();
            _levelManager.GetSoundManager.SoundSource.PlayOneShot(_levelManager.GetSoundManager.KickSound);
            _levelManager.GetPlayerPickUpAbilities.AddScore(enemy.fireballBonus, enemy.gameObject.transform.position);
            Debug.Log(this.name + " FireballTouchEnemy called on " + enemy.gameObject.name);
        }
    }
}
using System.Text.RegularExpressions;
using Core.Managers;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using ILevelManager = Interfaces.Core.Managers.ILevelManager;

namespace Level
{
    public class LevelServices : MonoBehaviour, ILevelServices
    {
        private ILevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public Vector3 FindSpawnPosition()
        {
            Vector3 spawnPosition;
            GameStateManager gameStateManager =
                GameObject.FindGameObjectWithTag("GameStateManager").GetComponent<GameStateManager>();
            Debug.Log(this.name + " FindSpawnPosition: GSM spawnFromPoint=" +
                      gameStateManager.SpawnFromPoint.ToString()
                      + " spawnPipeIdx= " + gameStateManager.SpawnPipeIdx.ToString()
                      + " spawnPointIdx=" + gameStateManager.SpawnPointIdx.ToString());
            if (gameStateManager.SpawnFromPoint) {
                spawnPosition = GameObject.Find("Spawn Points").transform.GetChild(gameStateManager.SpawnPointIdx)
                    .transform.position;
            } else {
                spawnPosition = GameObject.Find("Spawn Pipes").transform.GetChild(gameStateManager.SpawnPipeIdx)
                    .transform.Find("Spawn Pos").transform.position;
            }

            return spawnPosition;
        }

        public string GetWorldName(string sceneName)
        {
            string[] sceneNameParts = Regex.Split(sceneName, " - ");
            return sceneNameParts[0];
        }

        public bool IsSceneInCurrentWorld(string sceneName)
        {
            return GetWorldName(sceneName) == GetWorldName(SceneManager.GetActiveScene().name);
        }

        public void MarioCompleteCastle()
        {
            _levelManager.GetGameStateData.TimerPaused = true;
            _levelManager.GetSoundManager.GetSoundLevelHandle.ChangeMusic(
                _levelManager.GetSoundManager.CastleCompleteMusic);
            _levelManager.GetSoundManager.MusicSource.loop = false;
            _levelManager.GetPlayerController.GetMovement.AutomaticWalk(
                _levelManager.GetPlayerController.GetMovement.CastleWalkSpeedX);
        }

        public void MarioCompleteLevel()
        {
            _levelManager.GetGameStateData.TimerPaused = true;
            _levelManager.GetSoundManager.GetSoundLevelHandle.ChangeMusic(_levelManager.GetSoundManager
                .LevelCompleteMusic);
            _levelManager.GetSoundManager.MusicSource.loop = false;
        }

        public void MarioReachFlagPole()
        {
            _levelManager.GetGameStateData.TimerPaused = true;
            _levelManager.GetSoundManager.GetSoundLevelHandle.PauseMusicPlaySound(
                _levelManager.GetSoundManager.FlagpoleSound, false);
            _levelManager.GetPlayerController.GetMovement.ClimbFlagPole();
        }
    }
}
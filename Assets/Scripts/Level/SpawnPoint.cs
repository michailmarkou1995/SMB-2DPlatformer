﻿using Core.Managers;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
	public class SpawnPoint : MonoBehaviour {
		private IPlayerController _playerController;

		// Use this for initialization
		void Start () {
			_playerController = FindObjectOfType<IPlayerController> ();
		}
	
		// Update is called once per frame
		void Update () {
			// update spawn pos if Player passes checkpoint
			if (_playerController.gameObject.transform.position.x >= transform.position.x) {
				GameStateManager t_GameStateManager = FindObjectOfType<GameStateManager> ();
				t_GameStateManager.SpawnPointIdx = Mathf.Max (t_GameStateManager.SpawnPointIdx, gameObject.transform.GetSiblingIndex ());
				gameObject.SetActive (false);
			}

		}
	}
}

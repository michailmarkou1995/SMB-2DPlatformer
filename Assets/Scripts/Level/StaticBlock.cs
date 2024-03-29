﻿using Core.Managers;
using UnityEngine;

namespace Level
{
	public class StaticBlock : MonoBehaviour {
		private LevelManager t_LevelManager;

		// Use this for initialization
		void Start () {
			t_LevelManager = FindObjectOfType<LevelManager> ();
		}


		void OnCollisionEnter2D(Collision2D other) {
			Vector2 normal = other.contacts[0].normal;
			Vector2 bottomSide = new Vector2 (0f, 1f);
			bool bottomHit = normal == bottomSide;

			if (other.gameObject.tag == "Player" && bottomHit) {
				t_LevelManager.GetSoundManager.SoundSource.PlayOneShot (t_LevelManager.GetSoundManager.BumpSound);
			}

		}
	}
}

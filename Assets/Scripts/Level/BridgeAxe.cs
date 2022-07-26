using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using Core.NPC;
using Core.Player;
using UnityEngine;

namespace Level
{
	public class BridgeAxe : MonoBehaviour {
		private LevelManager t_LevelManager;
		private PlayerController playerController;
		private Bowser bowser;
		private List<GameObject> bridgePieces = new List<GameObject> ();

		private float bridgePieceGravity = 8;
		private float waitBetweenCollapse = .2f;
		private bool gotAxe;

		// Use this for initialization
		void Start () {
			t_LevelManager = FindObjectOfType<LevelManager> ();
			playerController = FindObjectOfType<PlayerController> ();
			bowser = FindObjectOfType<Bowser> ();

			foreach (Transform child in transform.parent.Find("Bridge Pieces")) {
				bridgePieces.Add (child.gameObject);
			}
		}


		void OnTriggerEnter2D(Collider2D other) {
			if (other.tag == "Player" && !gotAxe) {
				gotAxe = true;
				playerController.FreezeUserInput ();
				t_LevelManager.timerPaused = true;

				if (bowser) {  // bowser not yet defeated
					bowser.active = false; 
					StartCoroutine (CollapseBridgeCo ());
				} else {
					t_LevelManager.MarioCompleteCastle ();
				}
				gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
		}

		IEnumerator CollapseBridgeCo() {
			foreach (GameObject bridgePiece in bridgePieces) {
				bridgePiece.layer = LayerMask.NameToLayer ("Falling to Kill Plane");
				Rigidbody2D rgbd = bridgePiece.gameObject.GetComponent<Rigidbody2D> ();
				rgbd.bodyType = RigidbodyType2D.Dynamic;
				rgbd.gravityScale = bridgePieceGravity;
				t_LevelManager.GetSoundManager.SoundSource.PlayOneShot (t_LevelManager.GetSoundManager.BreakBlockSound);
				yield return new WaitForSeconds (waitBetweenCollapse);
			}
			t_LevelManager.MarioCompleteCastle ();
			Destroy (gameObject);
		}
	}
}

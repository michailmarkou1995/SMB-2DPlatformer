using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class BlockCoin : MonoBehaviour {
	private LevelManager t_LevelManager;

	// Use this for initialization
	void Start () {
		t_LevelManager = FindObjectOfType<LevelManager> ();
		t_LevelManager.AddCoin (transform.position + Vector3.down);
	}

}

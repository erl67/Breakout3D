﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class blockController : MonoBehaviour {

	Rigidbody rb;
	private int levelFlag;
	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();	
		switch (SceneManager.GetActiveScene().buildIndex)
		{
		case 0:
			levelFlag = 0;
			break;
		case 1:
			levelFlag = 1;
			break;
		case 2:
			levelFlag = 2;
			break;
		default:
			levelFlag = 3;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision other)
	{
		if (levelFlag != 0) {
			if (other.gameObject.tag.Equals ("ball") || other.gameObject.tag.Equals ("block")) {
				rb.constraints = RigidbodyConstraints.None;
			}
		}
		if (levelFlag == 0) {
			if (other.gameObject.tag.Equals ("ball")) {
				Destroy (gameObject);
			}
		}

		if (other.gameObject.tag.Equals ("bottom")) 
		{
			Destroy (gameObject);
		}

	}
}

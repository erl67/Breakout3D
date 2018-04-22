using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockController : MonoBehaviour {

	Rigidbody rb;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision other)
	{

		if (other.gameObject.tag.Equals ("ball")||other.gameObject.tag.Equals ("block")) 
		{
			rb = GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.None;
		}

		if (other.gameObject.tag.Equals ("bottom")) 
		{
			Destroy (gameObject);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockController : MonoBehaviour {

	Rigidbody rb;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
	{

		if (other.gameObject.tag.Equals ("ball")||other.gameObject.tag.Equals ("block")) 
		{
			rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
		}

		if (other.gameObject.tag.Equals ("bottom")) 
		{
			Destroy (gameObject);
		}
	}
}

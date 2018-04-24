using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class blockController : MonoBehaviour
{
    Rigidbody rb;
    private int levelFlag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        levelFlag = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (levelFlag == 0)
        {
            if (other.gameObject.tag.Equals("ball"))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag.Equals("ball") || other.gameObject.tag.Equals("block"))
            {
                rb.constraints = RigidbodyConstraints.None;
                //rb.constraints = RigidbodyConstraints.FreezePositionX;
            }
        }

        if (other.gameObject.tag.Equals("bottom"))
        {
            Destroy(gameObject);
        }
    }
}

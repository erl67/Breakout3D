using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour {
    private GameObject ball;
    private Rigidbody rb;
    private Vector3 force;
    private GameObject player;
    private Renderer mr;
    public Text txtScore, txtLives;

    public AudioSource blockHit;
	public AudioSource blockDied;
	public AudioSource bounce;//I changed the bounce sound effect here, which will make the structure easier --Yanbo

    private float ballScale, playerForce, playerRotation, blockForce, blockRotation;
    private Vector3 oppositeForce;

    void Start () {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                ballScale = 4f;
                playerForce = 3f;
                playerRotation = 1f;
                blockForce = 50f;
                blockRotation = 10f;
                break;
            case 1:

                break;
            case 2:
                break;
            default:
                break;
        }
        ball = this.gameObject;
        ball.transform.localScale = new Vector3(ballScale * Random.Range(.5f, 1f), ballScale * Random.Range(.5f, 1f), ballScale * Random.Range(.5f, 1f));

        rb = ball.GetComponent<Rigidbody>();
        mr = ball.GetComponent<Renderer>();
        player = GameObject.Find("Player").gameObject;
    }

    void Update () {
        // keep ball from bouncing out of game
        if (transform.position.z > Mathf.Abs(.5f)) transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BC with " + other.tag + " @ " + gameObject.transform.position);

        if (other.tag.Equals("block"))
        {
            int points = (int) other.GetComponent<Rigidbody>().mass;
            Debug.Log("block died : " + gameObject.transform.position + " " + points);
            blockHit.Play();

            mr.material.color *= other.GetComponent<Renderer>().material.color;

            force = new Vector3(Random.Range(-1 * blockForce, blockForce), 0f, 0) * 2f;

            rb.AddForce(force, ForceMode.Acceleration);

            rb.velocity *= .9f;
            //rb.angularVelocity = rb.angularVelocity * Random.Range(-1 * blockRotation, blockRotation);

            GameController.score += points;
            txtScore.text = "Score: " + GameController.score;

            Destroy(other.gameObject);
        }

        if (other.tag.Equals("player"))
        {
			bounce.Play ();
            force = new Vector3(0f, 10f, 0f) * 3f;
            rb.AddForce(force, ForceMode.Impulse);

            oppositeForce = player.GetComponent<Rigidbody>().velocity;
            Debug.Log("player velocity: " + oppositeForce + "  ball velocity: " + rb.velocity);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            force = new Vector3(0f, 10f, 0f) * playerForce;
            rb.AddForce(force, ForceMode.Impulse);

            GameController.score++;
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.tag.Equals("edge"))
        {
            GameController.score += -1;
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.tag.Equals("top"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (other.tag.Equals("bottom"))
        {
            mr.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            rb.transform.localScale *= .5f;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            //rb.isKinematic = true;
        }
    }

    private void OnBecameInvisible()
    {
        GameController.lives--;
        Destroy(gameObject);
    }
}

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
	private float constantSpeed = 30f;
	//private Vector3 startSpeed;
    public Text txtScore, txtLives;

    public AudioSource[] sounds;
    public AudioSource blockHit;
	public AudioSource blockDied;
	public AudioSource bounce;//I changed the bounce sound effect here, which will make the structure easier --Yanbo

    private float ballScale, playerForce, playerRotation, blockForce, blockRotation;
    private Vector3 oppositeForce;

    void Start () {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                ballScale = 3f;
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
        //ball.transform.localScale = new Vector3(ballScale * Random.Range(.5f, 1f), ballScale * Random.Range(.5f, 1f), ballScale * Random.Range(.5f, 1f));
        ball.transform.localScale *= ballScale;

        rb = ball.GetComponent<Rigidbody>();
        mr = ball.GetComponent<Renderer>();

        player = GameObject.Find("Player").gameObject;

        sounds = GetComponents<AudioSource>();
        //blockHit = null;
        //blockHit = sounds[0];
		force = new Vector3(2f, 10f, 0f) * 5f;
		rb.AddForce(force, ForceMode.Impulse);

    }

	void Update () //I commented the lines that restrict the ball from moving along z axis, but did the same thing in the Unity by checking "Freeze the position (z)"
	{
        //if (transform.position.z != 0f) transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        //if (transform.position.z > Mathf.Abs(.5f)) transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		//if (transform.position.z != 0f) transform.position.z = 0f;
		rb.velocity  = constantSpeed * (rb.velocity.normalized);
    }

    private void OnCollisionEnter(Collision other)
    {
		//Debug.Log("BC with " + other.collider.tag + " @ " + gameObject.transform.position);

		if (other.gameObject.tag.Equals("block"))
		{ 
			//!!!!I commented the next line. Because once I make the ball as a prefab, the text lose its attachment to the ball. Therefore, it stops the code from moving forward.
			//int points = (int) other.gameObject.GetComponent<Rigidbody>().mass;


            //Debug.Log("block died : " + gameObject.transform.position + " " + points);
            blockHit.Play();
            //int points = (int) other.GetComponent<Rigidbody>().mass;
            //blockHit.Play();
            //mr.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            //mr.material.color = other.GetComponent<Renderer>().material.color * .5f;
            //mr.material.color -= other.GetComponent<Renderer>().material.color;
            mr.material.color *= other.gameObject.GetComponent<Renderer>().material.color;


            //force = new Vector3(Random.Range(-1 * blockForce, blockForce), 0f, 0) * 2f;
            //force = new Vector3(blockForce, 0f, 0) * 2f; //need to find and set magnitude from edge

            //rb.AddForce(force, ForceMode.Acceleration);

            //rb.velocity *= .9f;
            //rb.angularVelocity = rb.angularVelocity * Random.Range(-1 * blockRotation, blockRotation);
			rb.velocity  = constantSpeed * (rb.velocity.normalized);

			//!!!!So do the next two lines as well
            //GameController.score += points;
            //txtScore.text = "Score: " + GameController.score;


			//blockDied.Play ();
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("player"))
        {
			bounce.Play ();
            //force = new Vector3(0f, 10f, 0f) * 3f;
            //rb.AddForce(force, ForceMode.Impulse);
            //oppositeForce = player.GetComponent<Rigidbody>().velocity;
            //Debug.Log("player velocity: " + oppositeForce + "  ball velocity: " + rb.velocity);

            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;

            //force = new Vector3(0f, 10f, 0f) * playerForce;
            //rb.AddForce(force, ForceMode.Impulse);
			rb.velocity  = constantSpeed * (rb.velocity.normalized);
            GameController.score++;
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.gameObject.tag.Equals("edge"))
        {
			rb.velocity  = constantSpeed * (rb.velocity.normalized);
            GameController.score += -1;
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.gameObject.tag.Equals("top"))
        {
			rb.velocity  = constantSpeed * (rb.velocity.normalized);
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
        }

        if (other.gameObject.tag.Equals("bottom"))
        {
            //mr.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			Destroy(gameObject);
            //rb.transform.localScale *= .5f;
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            //rb.useGravity = false;
            //rb.isKinematic = true;
        }
		rb.velocity  = constantSpeed * (rb.velocity.normalized);
    }

    private void OnBecameInvisible()
    {
        GameController.lives--;
        Destroy(gameObject);
    }
}

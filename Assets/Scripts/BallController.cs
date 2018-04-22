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
	private GameController Controller;

    public AudioSource blockHit;
	public AudioSource blockDied;
	public AudioSource bounce;//I changed the bounce sound effect here, which will make the structure easier --Yanbo

    private float ballScale, playerForce, playerRotation, blockForce, blockRotation;
    private Vector3 oppositeForce;

	GameObject topWall,leftWall,rightWall;
	MeshRenderer otherMr;
	int loopControl;

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
        ball.transform.localScale *= ballScale;


        rb = ball.GetComponent<Rigidbody>();
        mr = ball.GetComponent<Renderer>();
        player = GameObject.Find("Player").gameObject;

        force = new Vector3(2f, 10f, 0f) * 5f;
		rb.AddForce(force, ForceMode.Impulse);

		topWall = GameObject.FindWithTag("top");
		leftWall = GameObject.FindWithTag("left");
		rightWall = GameObject.FindWithTag("right");
        otherMr = topWall.GetComponent<MeshRenderer>();

        loopControl = 0;

		Controller = (GameController)GameObject.Find ("Main").GetComponent("GameController");

    }

	void Update () //I commented the lines that restrict the ball from moving along z axis, but did the same thing in the Unity by checking "Freeze the position (z)"
	{
		rb.velocity  = constantSpeed * (rb.velocity.normalized);
		if (loopControl == 15) 
		{
			otherMr.enabled = false;
			loopControl = 0;
		}
		loopControl++;
		//Debug.Log (loopControl);
    }

    private void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag.Equals("block"))
		{ 
			//var explo = Instantiate (explosion, new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), other.transform.rotation);
			//!!!!I commented the next line. Because once I make the ball as a prefab, the text lose its attachment to the ball. Therefore, it stops the code from moving forward.
			//int points = (int) other.gameObject.GetComponent<Rigidbody>().mass;


            //Debug.Log("block died : " + gameObject.transform.position + " " + points);
            blockHit.Play();

			//!!Random change a renderer, I comment it however
            //mr.material.color *= other.gameObject.GetComponent<Renderer>().material.color;

       

            //force = new Vector3(Random.Range(-1 * blockForce, blockForce), 0f, 0) * 2f;
            //force = new Vector3(blockForce, 0f, 0) * 2f; //need to find and set magnitude from edge


            //rb.AddForce(force, ForceMode.Acceleration);

            //rb.velocity *= .9f;
            //rb.angularVelocity = rb.angularVelocity * Random.Range(-1 * blockRotation, blockRotation);
			rb.velocity  = constantSpeed * (rb.velocity.normalized);

			//!!!!So do the next two lines as well
            //GameController.score += points;
            //txtScore.text = "Score: " + GameController.score;


            //Destroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("player"))
        {
			bounce.Play ();
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;

            //force = new Vector3(0f, 10f, 0f) * playerForce;
            //rb.AddForce(force, ForceMode.Impulse);
			rb.velocity  = constantSpeed * (rb.velocity.normalized);
        }

		if (other.gameObject.tag.Equals("left")||other.gameObject.tag.Equals("right"))
        {
			if(other.gameObject.tag.Equals("left"))
				otherMr = leftWall.GetComponent<MeshRenderer> ();
			else
				otherMr = rightWall.GetComponent<MeshRenderer> ();
			otherMr.enabled = true;
			rb.velocity  = constantSpeed * (rb.velocity.normalized);
        }

        if (other.gameObject.tag.Equals("top"))
        {
			otherMr = topWall.GetComponent<MeshRenderer> ();
			otherMr.enabled = true;
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

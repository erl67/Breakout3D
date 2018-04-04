using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {
    private GameObject ball;
    private Rigidbody rb;
    private Vector3 force;
    private GameObject player;

    public AudioSource[] sounds;
    public AudioSource blockHit;

    public Text txtScore, txtLives;

    void Start () {
        ball = this.gameObject;
        rb = ball.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;


        sounds = GetComponents<AudioSource>();
        blockHit = null;
        //blockHit = sounds[0];
    }

    void Update () {
        if (transform.position.z != 0f) transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("BC " + other.tag + " " + other.transform.position + " @ " + gameObject.transform.position);

        if (other.tag.Equals("block"))
        {
            int points = (int) other.GetComponent<Rigidbody>().mass;
            Debug.Log("block died : " + gameObject.transform.position + " " + points);
            //blockHit.Play();
            //int points = (int) other.GetComponent<Rigidbody>().mass;
            GameController.score += points;

            Destroy(other.gameObject);
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.tag.Equals("player"))
        {
            force = new Vector3(0f, 10f, 0f) * 3f;
            rb.AddForce(force, ForceMode.Impulse);

            Vector3 oppositeForce = player.GetComponent<Rigidbody>().velocity;

            GameController.score++;
            txtScore.text = "Score: " + GameController.score;
        }

        if (other.tag.Equals("edge"))
        {
            GameController.score+=99;
            txtScore.text = "Score: " + GameController.score;
        }
    }

    private void OnBecameInvisible()
    {
        GameController.lives--;
        Destroy(gameObject);
    }
}

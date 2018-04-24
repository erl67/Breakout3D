using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public GameObject ballPrefab;
    private Rigidbody rb;
    private GameController Controller;

    private int life;
    private int ballRemaining;
    private float timer = 5f;
    private float moveH, moveV, moveSpeed;
    private float playerScale, avScale;

    public Text txtScore, txtLives, txtCenter;
    public AudioSource loseLife, endSound, coin;

    void Start()
    {
        Controller = (GameController)GameObject.Find("Main").GetComponent("GameController");

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                moveSpeed = 1000f;
                playerScale = 3f;
                ballRemaining = 1;
                avScale = -10f;
                break;
            case 1:
                moveSpeed = 1000f;
                playerScale = 3f;
                ballRemaining = 2;
                avScale = -10f;
                break;
            case 2:
                moveSpeed = 1000f;
                playerScale = 3f;
                ballRemaining = 4;
                avScale = -10f;
                break;
            default:
                moveSpeed = Random.Range(2000f, 2500f);
                playerScale = Random.Range(3f, 6f);
                ballRemaining = Random.Range(6, 9);
                avScale = -120f;
                break;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;
        player.transform.localScale = new Vector3(playerScale, 1f, 1f);
    }

    public IEnumerator StartBox()   //called from NewLife()
    {
        while (!Input.anyKey)
        {
            if (timer < Time.realtimeSinceStartup) break;
            yield return null;
        }
        Controller.SetCenter("");

        Time.timeScale = 1;

        if (SceneManager.GetActiveScene().buildIndex < 2)
            player.transform.position = new Vector3(0f, 0f, 0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ballRemaining > 0) LaunchBall();

        if (Input.GetMouseButtonDown(2) && GameController.lives > 0) Controller.SetLives(-1);
        else if (Input.GetKeyDown(KeyCode.BackQuote)) Controller.SetLives(1);

        //restrict the posistion of the player, because the panel will occasionally move out of the playing area under extreme circumanstance.
        if (SceneManager.GetActiveScene ().buildIndex <= 2) {
			if (transform.position.x > 30f)
				transform.position = new Vector3 (30f, transform.position.y, 0f);
			if (transform.position.x < -30f)
				transform.position = new Vector3 (-30f, transform.position.y, 0f);
		} else { //this is a quick fix for last levels, may not always work
            if (transform.position.x > 30f)
                transform.position = new Vector3(transform.position.x - .75f, transform.position.y, 0f);
            if (transform.position.x < -30f)
                transform.position = new Vector3(transform.position.x + .75f, transform.position.y, 0f);
        }
    }

    void FixedUpdate()
    {
        float mouseH = Input.GetAxis("Mouse X");

        if (mouseH != 0f)
        {
            Vector3 motion = new Vector3(mouseH * moveSpeed, 0f, 0f);
            var force = motion * moveSpeed;
            rb.AddForce(force);
            //Debug.Log("mouseH: " + mouseH + " motion: " + motion + " v: " + rb.velocity);

            if (!Input.GetKeyDown(KeyCode.F)) rb.velocity = Vector3.zero;
        }
    }

    public void LaunchBall()
    {
        if (GameObject.FindGameObjectsWithTag("ball").Length < 3 && Time.timeScale > 0)
        {
            var ball = Instantiate(ballPrefab) as GameObject;
            ball.transform.position = (transform.position + new Vector3(0f, Random.Range(3f, 3.5f), 0f));
            ballRemaining--;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("block"))
        {
            //make block spin
            var angular = other.gameObject.GetComponent<Rigidbody>().angularVelocity;
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = angular * Random.Range(avScale, Mathf.Abs(avScale));

            int points = (int)other.gameObject.GetComponent<Rigidbody>().mass;
            Controller.AddScore(points);
            coin.Play();
        }

        if (other.gameObject.tag.Equals("left") || other.gameObject.tag.Equals("right"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void LoseLife()
    {
        loseLife.Play();

        //reset balls available for each level
        ballRemaining = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.GetActiveScene().buildIndex == 2) ballRemaining = 4;
        if (SceneManager.GetActiveScene().buildIndex > 2) ballRemaining = 6;

        //only levels 1-3 will pause when ball is lost
        if (SceneManager.GetActiveScene().buildIndex < 2)
        {
            Time.timeScale = 0;
            Controller.SetCenter("\nYou dropped the ball.\nPress ( AnyKey ) to continue");
            NewLife();
        }
    }

    public void NewLife()
    {
        timer = Time.realtimeSinceStartup + 3f;
        StartCoroutine(StartBox());
    }
}

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
    private float newScale, timer = 5f;
    private float moveH, moveV, moveSpeed;
    private float playerScale, avScale;

    public AudioSource loseLife, endSound, coin;

    public Text txtScore, txtLives, txtCenter;

    void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                moveSpeed = 1500f;
                playerScale = 3f;
                ballRemaining = 1;
                avScale = -10f;
                break;
            case 1:
                moveSpeed = 1200f;
                playerScale = 2.7f;
                ballRemaining = 2;
                avScale = -10f;
                break;
            case 2:
                moveSpeed = 1500f;
                playerScale = 2.5f;
                ballRemaining = 4;
                avScale = -10f;
                break;
            default:
                moveSpeed = Random.Range(1500f, 2000f);
                playerScale = Random.Range(3f, 6f);
                ballRemaining = 5;
                avScale = -100f;
                break;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;
        player.transform.localScale = new Vector3(playerScale, 1f, 1f);

        Controller = (GameController)GameObject.Find("Main").GetComponent("GameController");

        life = GameController.lives;

        newScale = 30f - (playerScale / 3.0f);

        //yield return new WaitForSecondsRealtime(2);
    }

    public IEnumerator StartBox()   //called from NewLife()
    {
        //yield return new WaitForSecondsRealtime(1);

        while (!Input.anyKey)
        {
            if (timer < Time.realtimeSinceStartup) break;
            yield return null;
        }
        Controller.SetCenter(" ");

        Time.timeScale = 1;
        player.transform.position = new Vector3(0f, 0f, 0f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ballRemaining != 0)
        {
            LaunchBall();
        }
        if (life != GameController.lives)
        {
            life = GameController.lives;
            LoseLife();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameController.lives--;
            LoseLife();
        }

        //if (Mathf.Abs(transform.position.x) > newScale) transform.position = new Vector3(newScale, transform.position.y, 0f);

        //if (Mathf.Abs(transform.position.x) > 30f) transform.position = new Vector3(30f, transform.position.y, 0f);
        //!!This is for restrict the posistion of the player, because the panel will occasionally move out of the playing area under extream circumanstance.
        //If the scale of the panel changes, these two lines need to be fixed also.
    }

    void FixedUpdate()
    {
        float mouseH = Input.GetAxis("Mouse X");
        //float mouseV = Input.GetAxis("Mouse Y");

        if (mouseH != 0f)
        {
            Vector3 motion = new Vector3(mouseH * moveSpeed, 0f, 0f);
            var force = motion * moveSpeed;
            rb.AddForce(force);
            Debug.Log("mouseH: " + mouseH + " force: " + motion + " v: " + rb.velocity);
            if (!Input.GetKeyDown(KeyCode.F))
                rb.velocity = Vector3.zero;
        }
    }

    public void LaunchBall()
    {
        if (GameObject.FindGameObjectsWithTag("ball").Length < 3)
        {
            var ball = Instantiate(ballPrefab) as GameObject;
            ball.transform.position = (transform.position + new Vector3(0f, 3f, 0f));
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
        int lives = GameController.lives;

        if (lives < 1)
            endSound.Play();
        else
            loseLife.Play();

        Time.timeScale = 0;

        if (lives < 1)
        {
            Controller.SetCenter("Game Over\nYour Final Score is: " + GameController.score.ToString() + "\n\nPress (r) to try again");
            GameController.instance.PlayerDead();
        }
        else
        {
            Controller.SetCenter("\nYou dropped the ball.\nPress ( AnyKey ) to continue");
            NewLife();
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ballRemaining = 1;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            ballRemaining = 2;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            ballRemaining = 4;
        }
    }

    public void NewLife()
    {
        timer = Time.realtimeSinceStartup + 3f;
        StartCoroutine(StartBox());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public GameObject player;
    private Rigidbody rb;

    private int life;
    private float timer = 1f;
    private float moveH, moveV, moveSpeed;

    public AudioSource[] sounds;
    public AudioSource bounce, loseLife, endSound;

    public Text txtScore, txtLives, txtCenter;

    IEnumerator Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                moveSpeed = 100f;
                break;
            case 1:
                moveSpeed = 200f;
                break;
            case 2:
                break;
            default:
                break;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;
        life = GameController.lives;

        txtLives.text = "Lives: " + life;
        txtScore.text = "Score: " + GameController.score;

        sounds = GetComponents<AudioSource>();
        bounce = loseLife = endSound = null;

        //bounce = sounds[0];
        //loseLife = sounds[1];
        //endSound = GameObject.Find("GameOver").gameObject.GetComponent<AudioSource>();

        yield return new WaitForSecondsRealtime(2);
    }

    public IEnumerator StartBox()   //called from NewLife()
    {
        yield return new WaitForSecondsRealtime(1);

        while (!(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        txtCenter.text = "";

        Time.timeScale = 1;
        player.transform.position = new Vector3(0f, 0f, 0f);
    }

    void Update () {

        if (life != GameController.lives)
        {
            txtLives.text = "LIves: " + GameController.lives;
            life = GameController.lives;
            LoseLife();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameController.lives--;
            LoseLife();
        }
    }

    void FixedUpdate()
    {
        float mouseH = Input.GetAxis("Mouse X");
        float mouseV = Input.GetAxis("Mouse Y");

        if (mouseH != 0f || mouseV != 0f)
        {
            Vector3 motion = new Vector3(mouseH * moveSpeed, 0f, 0f);
            rb.AddForce(motion * 1000f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(-100f, 0f, 0f) * 500f, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(new Vector3(100f, 0f, 0f) * 500f, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("edge"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("edge"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnBecameInvisible()
    {
        GameController.lives--;
        LoseLife();
     }


    public void LoseLife()
    {
        //lives--;
        //txtLives.text = "Lives: " + lives;

        int lives = GameController.lives;

        //if (lives < 1) { endSound.Play(); }
        //else { loseLife.Play(); }

        Time.timeScale = 0;

        if (lives < 1)
        {
            txtCenter.text = "Game Over\nYour Final Score is: " + GameController.score.ToString();
            txtCenter.text += "\n\nPress (r) to try again";
            GameController.instance.PlayerDead();
        }
        else
        {
            txtCenter.text = "\nYou dropped the ball.\nPress (r or space) to continue";
            NewLife();
        }
    }

    public void NewLife()
    {
        //var blocks = GameObject.FindGameObjectsWithTag("block"); //on next level
        //foreach (GameObject block in blocks) Destroy(block);
        StartCoroutine(StartBox());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    private GameObject ball;
    private Rigidbody rb;
    private Vector3 force;
    private GameObject player;
    private Renderer mr;
    private float constantSpeed = 30f;
    private GameController Controller;

    private int levelFlag;

    public AudioSource blockHit;
    public AudioSource blockDied;
    public AudioSource coin;
    public AudioSource bounce;//I changed the bounce sound effect here, which will make the structure easier --Yanbo

    private float ballScale, playerForce, playerRotation, blockForce, blockRotation;
    private Vector3 oppositeForce;

    GameObject topWall, leftWall, rightWall;
    MeshRenderer otherMr;
    int loopControl;

    void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                ballScale = 3f;
                playerForce = 3f;
                playerRotation = 1f;
                blockForce = 50f;
                blockRotation = 10f;
                levelFlag = 0;
                break;
            case 1:
                ballScale = 3f;
                playerForce = 3f;
                playerRotation = 1f;
                blockForce = 50f;
                blockRotation = 10f;
                levelFlag = 1;
                break;
            case 2:
                ballScale = 3f;
                playerForce = 3f;
                playerRotation = 1f;
                blockForce = 50f;
                blockRotation = 10f;
                levelFlag = 2;
                break;
            default:
                ballScale = 3f;
                playerForce = 3f;
                playerRotation = 1f;
                blockForce = 50f;
                blockRotation = 10f;
                levelFlag = 3;
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

        Controller = (GameController)GameObject.Find("Main").GetComponent("GameController");
    }

    void Update() //I commented the lines that restrict the ball from moving along z axis, but did the same thing in the Unity by checking "Freeze the position (z)"
    {
        rb.velocity = constantSpeed * (rb.velocity.normalized);
        if (loopControl == 15)
        {
            otherMr.enabled = false;
            loopControl = 0;
        }
        loopControl++;
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag.Equals("block"))
        {
            //int points = (int) other.gameObject.GetComponent<Rigidbody>().mass;

            blockHit.Play();

            //change color of ball based on block hit
            if (SceneManager.GetActiveScene().buildIndex > 2)
            {
                mr.material.mainTexture = null;

                if (Random.Range(0, 9) > 1)
                    mr.material.color *= other.gameObject.GetComponent<Renderer>().material.color;
                else
                    mr.material.color = new Color(Random.Range(.8f,1f), Random.Range(.8f,1f), Random.Range(.8f,1f));
            }

            if (levelFlag == 0)
            {
                var score = (int)other.gameObject.GetComponent<Rigidbody>().mass;
                Controller.AddScore(score);
                coin.Play();
            }
        }

        if (other.gameObject.tag.Equals("player"))
        {
            bounce.Play();
            //var power = other.gameObject.GetComponent<Rigidbody>().mass;
            //Debug.Log(power);
        }

        if (other.gameObject.tag.Equals("left") || other.gameObject.tag.Equals("right"))
        {
            if (other.gameObject.tag.Equals("left"))
                otherMr = leftWall.GetComponent<MeshRenderer>();
            else
                otherMr = rightWall.GetComponent<MeshRenderer>();
            otherMr.enabled = true;
        }

        if (other.gameObject.tag.Equals("top"))
        {
            otherMr = topWall.GetComponent<MeshRenderer>();
            otherMr.enabled = true;
        }

        if (other.gameObject.tag.Equals("bottom"))
        {
            BallDies();
        }
    }

    private void BallDies()
    {
        Destroy(gameObject);
        Controller.SetLives(-1);
    }
}

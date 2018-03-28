using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public GameObject player;
    private Rigidbody rb;
    private Transform target;
    private RaycastHit hit;
    private Ray mouseClick;
    private Vector3 cursorPosition;
    private float timer = 1f;

    //    public Maze mazeInstance;

    public AudioSource[] sounds;
    public AudioSource bounce, loseLife, endSound;
    private int score = 0, lives = 3;
    public Text txtScore, txtLives, txtCenter;

    IEnumerator Start()
    {
        //agent = gameObject.GetComponent<NavMeshAgent>();
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;

        txtLives.text = "Lives: " + lives;
        txtScore.text = "Score: " + score;

        sounds = GetComponents<AudioSource>();
        bounce = loseLife = endSound = null;

        //bounce = sounds[0];
        //loseLife = sounds[1];
        //endSound = GameObject.Find("GameOver").gameObject.GetComponent<AudioSource>();

        yield return new WaitForSecondsRealtime(2);
        //mazeInstance = GameObject.Find("mazeInstance").GetComponent<Maze>();

        //AgentOn();
    }

    public IEnumerator StartBox()
    {
        yield return new WaitForSecondsRealtime(1);

        while (!(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        txtCenter.text = "";

        //mazeInstance.ToggleMaze();
        //GameController.instance.MakeGhosts();

        Time.timeScale = 1;
        player.transform.position = new Vector3(0f, 0f, 0f);
        //AgentOn();
    }

    void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            LoseLife();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PC " + other.tag + " " + other.transform.position + " @ " + gameObject.transform.position);
        if (other.tag.Equals("ball"))
        {
            bounce.Play();
            Destroy(other.gameObject);
            score++;
            txtScore.text = "Score: " + score;
        }
        if (other.tag.Equals("block"))
        {
            Debug.Log(" died : " + gameObject.transform.position);
            Destroy(other.gameObject);
            LoseLife();
        }
    }

    private void LoseLife()
    {
        lives--;
        txtLives.text = "Lives: " + lives;

        if (lives < 1) { endSound.Play(); }
        else { loseLife.Play(); }

        Time.timeScale = 0;

        float intWall = 7.5f - lives;
        float extWall = 14.5f + lives;

        if (lives < 1)
        {
            txtCenter.text = "Game Over\nYour Final Score is: " + score.ToString();
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
        //var ghosts = GameObject.FindGameObjectsWithTag("ghost");
        //foreach (GameObject ghost in ghosts) Destroy(ghost);
        StartCoroutine(StartBox());
    }
}




//    void FixedUpdate () {

//        if (Input.GetMouseButton(0) && agent.isActiveAndEnabled)
//        {
//            mouseClick = Camera.main.ScreenPointToRay(Input.mousePosition);
//            if (Physics.Raycast(mouseClick, out hit, 1000))
//            {
//                cursorPosition = hit.point;
//                NavMesh.SamplePosition(cursorPosition, out navHitPosition, 15f, 1 << NavMesh.GetAreaFromName("Walkable"));
//                agent.SetDestination(navHitPosition.position);
//                Debug.Log("Pacman moving to " + agent.destination);
//            }         
//        }

//        if (Input.GetKeyDown(KeyCode.M))
//        {
//            float mouseH = Input.GetAxis("Mouse X");
//            float mouseV = Input.GetAxis("Mouse Y");
//            if (mouseH != 0f || mouseV != 0f)
//            {
//                rb.isKinematic = false;
//                Vector3 motion = new Vector3(mouseH, 3f, mouseV);
//                rb.AddForce(motion * 5f);
//                Invoke("AgentOn", 3);
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.T) && agent.isActiveAndEnabled)
//        {
//            agent.enabled = false;
//            rb.isKinematic = false;
//            rb.AddForce(new Vector3(0f, 4f, 8f) * 2f, ForceMode.Impulse);
//            Invoke("AgentOn", 1);
//        }
//        if (Input.GetKeyDown(KeyCode.G) && agent.isActiveAndEnabled)
//        {
//            agent.enabled = false;
//            rb.isKinematic = false;
//            rb.AddForce(new Vector3(0f, 4f, -8f) * 2f, ForceMode.Impulse);
//            Invoke("AgentOn", 1);
//        }
//        if (Input.GetKeyDown(KeyCode.F) && agent.isActiveAndEnabled)
//        {
//            agent.enabled = false;
//            rb.isKinematic = false;
//            rb.AddForce(new Vector3(-8f, 4f, 0f) * 2f, ForceMode.Impulse);
//            Invoke("AgentOn", 1);
//        }
//        if (Input.GetKeyDown(KeyCode.H) && agent.isActiveAndEnabled)
//        {
//            agent.enabled = false;
//            rb.isKinematic = false;
//            rb.AddForce(new Vector3(8f, 4f, 0f) * 2f, ForceMode.Impulse);
//            Invoke("AgentOn", 1);
//        }
//        if (Input.GetKeyDown(KeyCode.B) && agent.isActiveAndEnabled)
//        {
//            agent.enabled = false;
//            rb.isKinematic = false;
//            rb.AddForce(new Vector3(Random.Range(-3f, 3f), 6f, Random.Range(-3f, 3f)) * 5f, ForceMode.Impulse);
//            Invoke("AgentOn", 3);
//        }
//        if (Input.GetKeyDown(KeyCode.V))
//        {
//            AgentOn();
//        }
//    }

//    private void AgentOn()
//    {
//        agent.enabled = true;
//        rb.isKinematic = true;
//        agent.Warp(player.transform.position);
//        //agent.ResetPath();
//    }

//    


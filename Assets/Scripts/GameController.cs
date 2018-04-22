using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool gameOver;
    public bool spawn = true, spawnBall = false;
    //private AudioSource background;

    private float volume, timer, timer2;
    public Text txtScore, txtLives, txtCenter, txtHelp;

    public static int score = 0, lives = 3;

    private int blocksRemaing = 100;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        gameOver = false;
    }

    void Start()
    {
		lives = 3;
        AudioListener.volume = .2f;

        StartCoroutine(StartBox());
        Time.timeScale = 0;
        txtCenter.text = "Press any Key to Begin";
        BeginGame();

		txtScore.text = "Score: " + score;
		txtLives.text = "Lives: " + lives;

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                timer = Time.time + 15;
                break;
            case 1:
                timer = Time.time + 15;
                timer2 = Time.time + 30;
                break;
            default:
                break;
        }

        BeginGame();
    }

    private void BeginGame()
    {
    }

    public IEnumerator StartBox()
    {
        while (!Input.anyKey)
        {
            yield return null;
        }
        txtHelp.text = "";
        txtCenter.text = "";
        Time.timeScale = 1;
        GameObject.Find("Player").transform.position = new Vector3(0f, 0f, 0f);
    }

	public void AddScore(int delta)
	{
		score += delta;
		txtScore.text = "Score: " + score;
	}

    public int GetScore()
    {
        return score;
    }

    public void SetLives(int delta)
    {
        lives += delta;
        txtLives.text = "Lives: " + lives;
    }

    public void SetCenter(string s)
    {
        if (txtHelp != null) txtCenter.text = s;
    }

    public void SetHelp(string s)
    {
        if (txtHelp != null) txtHelp.text = s;
    }

    void Update()
    {
        blocksRemaing = GameObject.FindGameObjectsWithTag("block").Length; //on next level
        //foreach (GameObject block in blocks) Destroy(block);

        if (GameController.instance.gameOver)
        {
            PlayerDead();
        }

        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);
            GameObject.Find("OverheadLight").gameObject.GetComponent<Light>().enabled = false;
            GameObject.Find("Spotlight").gameObject.GetComponent<Light>().enabled = true;
        }

        if (blocksRemaing < 10 && Input.GetKeyDown(KeyCode.N))
        {
            //PlayerDead();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.C))))
        {
            PlayerDead();
            UnityEditor.EditorApplication.isPlaying = false;  //hide for build
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Pause))
        {
            volume = Time.timeScale == 1 ? AudioListener.volume : volume;
            Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            spawn = spawn == true ? false : true;
            AudioListener.volume = Time.timeScale == 0 ? 0f : volume;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.H))
        {
            GameObject.Find("Player").GetComponent<SphereCollider>().enabled = false;
        }


        if (timer < Time.time)
        {
            timer = Time.time + Random.Range(1f, 3f);
            spawnBall = true;
        }

        if (timer2 < Time.time)
        {
            timer2 = Time.time + Random.Range(5f, 10f);
        }

        if (spawnBall)
        {
            Debug.Log("Spawning ball");
            spawnBall = false;
        }

    }

    public void MuteBG()
    {
        //background.mute = true;
    }

    public void PlayerDead()
    {
        //StopAllCoroutines();

        MuteBG();
        spawn = false;
        gameOver = true;
    }

}
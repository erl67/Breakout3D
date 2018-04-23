using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool gameOver;

    private float volume, timer, timer2;
    public Text txtScore, txtLives, txtCenter, txtHelp;

    public static int score = 0, lives = 5;

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
        AudioListener.volume = .2f;

        StartCoroutine(StartBox());
        Time.timeScale = 0;
        txtCenter.text = "Press any Key to Begin";

		txtScore.text = "Score: " + score;
		txtLives.text = "Lives: " + lives;

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
			lives = 5;
			score = 0;
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

    }

    public void PlayerDead()
    {
        //StopAllCoroutines();
        gameOver = true;
    }
}
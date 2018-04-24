using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool gameOver;
    private CameraController view;
    private PlayerController player;

    public Text txtScore, txtLives, txtCenter, txtHelp;

    private int blocksRemaining = 999;
    private float volume, timer;

    public static int score = 0, lives = 5;

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
        view = (CameraController)GameObject.Find("MainCamera").GetComponent("CameraController");
        player = (PlayerController)GameObject.Find("Player").GetComponent("PlayerController");

        AudioListener.volume = .25f;
        timer = Time.realtimeSinceStartup + 5f;

        txtHelp.text = "";
        txtCenter.text = "Press any Key to Begin";
        txtScore.text = "Score: " + score;
        txtLives.text = "Lives: " + lives;
        StartCoroutine(StartBox());
    }

    public IEnumerator StartBox()
    {
        while (!Input.anyKey)
        {
            if (timer < Time.realtimeSinceStartup) break;
            yield return null;
        }
        txtCenter.text = "";
        Time.timeScale = 1;
        GameObject.Find("Player").transform.position = new Vector3(0f, 0f, 0f);
    }

    public void AddScore(int delta)
    {
        if (view.IsMoving()) delta = (int) (delta * 1.5);
        score += delta;
        txtScore.text = "Score: " + score;
    }

    public int GetScore()
    {
        return score;
    }

    public void SetLives(int delta)
    {
        if (delta < 0 && SceneManager.GetActiveScene().buildIndex !=4) view.ResetCamera();
        lives += delta;
        txtLives.text = "Lives: " + lives;

        if (lives <= 0) {
            player.endSound.Play();
            GameController.instance.PlayerDead();
        }
        else if (delta < 0) player.LoseLife();
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
        if (GameController.instance.gameOver) PlayerDead();

        blocksRemaining = GameObject.FindGameObjectsWithTag("block").Length;

        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            lives = 5;
            score = 0;
            GameObject.Find("OverheadLight").gameObject.GetComponent<Light>().enabled = true;
            GameObject.Find("Spotlight").gameObject.GetComponent<Light>().enabled = true;
            SceneManager.LoadScene(0);
        }

        if (blocksRemaining < 5 && Input.anyKey)
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (blocksRemaining < 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (blocksRemaining < 6)
        {
            SetCenter("Press any key to advance");
        }

        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.C))))
        {
            PlayerDead();
            //UnityEditor.EditorApplication.isPlaying = false;  //hide for build
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
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void PlayerDead()
    {
        //StopAllCoroutines();
        Time.timeScale = 0;
        SetCenter("Game Over\nYour Final Score is: " + score.ToString() + "\n\nPress (r) to try again");
        gameOver = true;
    }
}

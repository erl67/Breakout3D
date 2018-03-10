using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool gameOver;
    public bool spawn = true, spawnBall = false;

    private AudioSource background;

    private float volume, timer, timer2;

    public Maze mazePrefab;

    private Maze mazeInstance;

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
        background = GetComponent<AudioSource>();   //background music

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                timer = Time.time + 3;
                break;
            case 1:
                timer = Time.time + 2;
                timer2 = Time.time + 3;
                break;
            default:
                break;
        }

        BeginGame();
    }

    private void BeginGame()
    {
        Debug.Log("Beginning Game");
        mazeInstance = Instantiate(mazePrefab) as Maze;
        StartCoroutine(mazeInstance.Generate());

    }

    void Update()
    {
        if (GameController.instance.gameOver)
        {
            PlayerDead();
        }

        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!spawn && Input.GetKeyDown(KeyCode.N))
        {
            PlayerDead();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
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

        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioListener.volume = AudioListener.volume * .9f;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AudioListener.volume = AudioListener.volume * 1.1f;
        }

        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.H))
        {
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            SceneManager.LoadScene(0);
        }

        if (timer < Time.time)
        {
            timer = Time.time + Random.Range(1f, 3f);
            spawnBall = true;
        }

        if (timer2 < Time.time)
        {
            timer2 = Time.time + Random.Range(2f, 4f);
        }

        if (spawnBall)
        {

        }

    }

    void OnBecameInvisible()
    {
        Debug.Log("GC Invisibile" + this.tag);
    }

    public void MuteBG()
    {
        background.mute = true;
    }

    public void PlayerDead()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);

        MuteBG();
        spawn = false;
        gameOver = true;
    }

}

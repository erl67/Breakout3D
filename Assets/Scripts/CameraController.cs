using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameController Controller;
    public GameObject player;
    private Vector3 offset;

    private float x, y, moveH, moveV;
    private Vector3 rotateValue;

    public float minFov = 15f, maxFov = 120f, sensitivity = 50f;
    public float fov, fovStart;

    public Vector3 camStartP;
    public Quaternion camStartR;

    public Light overhead, spotlight;

    //private Rigidbody player;
    private Rigidbody ball;

    private bool rotateLeft = false;
    private bool rotateRight = false;
    private int score = 0;

    void Start()
    {
        Controller = (GameController)GameObject.Find("Main").GetComponent("GameController");
        overhead = GameObject.Find("OverheadLight").gameObject.GetComponent<Light>();
        spotlight = GameObject.Find("Spotlight").gameObject.GetComponent<Light>();
        spotlight.enabled = true;
        overhead.enabled = false;

        //player = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();

        offset = transform.position - player.transform.position;
        Debug.Log("Camera: " + transform.position + "  Player: " + player.transform.position);

        fov = Camera.main.fieldOfView;
        camStartP = Camera.main.transform.position;
        camStartR = Camera.main.transform.rotation;
        fovStart = fov;
    }

    void Update()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        score = Controller.GetScore();

        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space))
        {
            ResetCamera();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            rotateLeft = rotateLeft == false ? true : false;
            rotateRight = false;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            rotateRight = rotateRight == false ? true : false;
            rotateLeft = false;
        }
        else if (score % 20 == 0)
        {
            rotateLeft = true;
            rotateRight = false;
        }
        else if (score % 10 == 0)
        {
            rotateLeft = false;
            rotateRight = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var None = spotlight.enabled == false ? spotlight.enabled = true : spotlight.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var None = overhead.enabled == false ? overhead.enabled = true : overhead.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            LightSwitch();
        }
    }

    void LateUpdate()
    {
        if (rotateLeft == true)
        {
            transform.RotateAround(player.transform.position, Vector3.up, 10f * Time.deltaTime);
        }
        else if (rotateRight == true)
        {
            transform.RotateAround(player.transform.position, Vector3.down, 10f * Time.deltaTime);
        }

        rotateValue = new Vector3(moveV, moveH * -1, 0);    //move camera with WASD
        transform.eulerAngles = transform.eulerAngles - rotateValue;

        //uncomment for camera follow player
        //transform.position = player.transform.position + offset;

        fov += (Input.GetAxis("Mouse ScrollWheel") * -50f);

        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    public static void LightSwitch()
    {
        var over = GameObject.Find("OverheadLight").gameObject.GetComponent<Light>();
        var spot = GameObject.Find("Spotlight").gameObject.GetComponent<Light>();
        var None = spot.enabled == false ? spot.enabled = over.enabled = true : spot.enabled = over.enabled = false;
    }

    public void ResetCamera()
    {
        Camera.main.fieldOfView = 90;
        Camera.main.transform.position = camStartP;
        Camera.main.transform.rotation = camStartR;
    }
}

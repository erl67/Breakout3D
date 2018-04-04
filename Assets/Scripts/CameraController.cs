using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    private float x, y, moveH, moveV;
    private Vector3 rotateValue;

    public float minFov = 15f, maxFov = 120f, sensitivity = 5f;
    public float fov, fovStart;

    public Vector3 camStartP;
    public Quaternion camStartR;

    public Light overhead, spotlight;

    void Start()
    {
        overhead = GameObject.Find("OverheadLight").gameObject.GetComponent<Light>();
        spotlight = GameObject.Find("Spotlight").gameObject.GetComponent<Light>();
        spotlight.enabled = true;
        overhead.enabled = false;

        offset = transform.position - player.transform.position;
        Debug.Log("Camera: " + transform.position + "  Player: " + player.transform.position);

        fov = Camera.main.fieldOfView;
        camStartP = Camera.main.transform.position;
        camStartR = Camera.main.transform.rotation;
        fovStart = fov;
    }

    void LateUpdate()
    {
        //transform.position = player.transform.position + offset;

        //https://answers.unity.com/questions/218347/how-do-i-make-the-camera-zoom-in-and-out-with-the.html
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    void Update()
    {
        //https://answers.unity.com/questions/1179680/how-to-rotate-my-camera.html
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        rotateValue = new Vector3(moveV, moveH * -1, 0);
        transform.eulerAngles = transform.eulerAngles - rotateValue;

        if (Input.GetMouseButtonDown(2))
        {
            ResetCamera();
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

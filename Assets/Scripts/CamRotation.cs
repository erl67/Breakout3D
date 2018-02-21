using UnityEngine;
using System.Collections;

//https://answers.unity.com/questions/1179680/how-to-rotate-my-camera.html

public class CamRotation : MonoBehaviour
{
    private float x, y, moveH, moveV;
    private Vector3 rotateValue;

    void Update()
    {
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");

        rotateValue = new Vector3(x, y * -1, 0);
        transform.eulerAngles = transform.eulerAngles - rotateValue;

        rotateValue = new Vector3(moveV, moveH * -1, 0);
        transform.eulerAngles = transform.eulerAngles - rotateValue;

        //Debug.Log(x + ":" + y + "   " + moveH + ":" + moveV);
    }
}

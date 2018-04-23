using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System;
using System.Linq;

public class ArduinoController : MonoBehaviour
{
    // SerialPortConnectorMac portConnect;
    public static SerialPort sp;
    public static string strIn;
    public static List<string> portList;
    public float smooth = 2.0F;
    private Vector3 prevPosition;

    private bool useController = true, gyroOnly = false, ballLoaded = true;
    private float timer = 20f;

    private CameraController view;
    private PlayerController paddle;
    private Rigidbody player;
    private Rigidbody ball;
    private Vector3 force;

    //private int ax = 0 , ay = 0, gx = 0, gy = 0;
    private int moveX = 0, moveY = 0;
    private int turnX = 0, turnY = 0;

    private int axThreshold = 30, ayThreshold = 50; //move, not used
    private int gxThreshold = 30; //move
    private int gyThreshold = 75, gyThresholdNeg = -40; //laucnh ball, resetCamera

    public float moveScale = 50f;
    public float moveGyroScale = 500f;
    public float powerScale = 10f;

    void MovementData(string s)
    {
        if (s.Substring(0, 4) == "a/g:")
        {
            s = s.Replace("a/g:\t", "");
            string[] arduinoData = s.Split('\t');

            if (arduinoData.Length == 4)
            {
                moveX = int.Parse(arduinoData[0]);   //inverted in sketch
                moveY = int.Parse(arduinoData[1]);   //see x/y marked on chip
                turnX = int.Parse(arduinoData[2]);
                turnY = int.Parse(arduinoData[3]);

                Debug.Log("accl (x:" + moveX + ",  y:" + moveY + ")\t gyro (x: " + turnX + ",  y:" + turnY + ")");
            }
            else
            {
                moveX = moveY = turnX = turnY = 0;
                //player.velocity = Vector3.zero;
            }

            if (gyroOnly)
            {
                if (Mathf.Abs(turnX) > gxThreshold)
                {
                    force = turnX > 0 ? new Vector3(moveGyroScale, 0f, 0f) : new Vector3(-1 * moveGyroScale, 0f, 0f);

                    player.AddForce(force * 50f, ForceMode.VelocityChange);

                    Debug.Log("gyro force: " + force.ToString() + "\tvelocity: " + player.velocity);
                }
            }
            else
            {
                if (Mathf.Abs(moveX) > axThreshold)
                {
                    force = new Vector3(moveX * moveScale, 0f, 0f);
                    player.AddForce(force, ForceMode.VelocityChange);
                    Debug.Log("move force: " + force.ToString() + "\tvelocity: " + player.velocity);

                    //UnityEditor.EditorApplication.isPlaying = false;
                }
            }

            if (turnY > gyThreshold && ballLoaded)   //resetCamera
            {
                paddle.LaunchBall();
                //player.mass = (int)(100 + turnY);
                ballLoaded = false;
                turnY = 0;
            }
            else if (turnY < gyThresholdNeg)
            {
                view.ResetCamera();
                turnY = 0;
            }
        }
        //else Debug.Log("data-> " + data);            

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            gyroOnly = gyroOnly == false ? true : false;
            player.transform.localScale = gyroOnly ? player.transform.localScale * .75f : player.transform.localScale * 1.25f;
            Debug.Log("changing mode  gyroOnly = " + gyroOnly);
        }

        if (GameObject.FindGameObjectsWithTag("ball").Length == 0 || (timer < Time.time && !ballLoaded))
        {
            ballLoaded = true;
            timer = Time.time + 20f;
        }
    }

    void Start()
    {
        player = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();
        view = (CameraController)GameObject.Find("MainCamera").GetComponent("CameraController");
        paddle = (PlayerController)GameObject.Find("Player").GetComponent("PlayerController");

        if (useController)
        {
            string port = null;
            var pLength = System.IO.Ports.SerialPort.GetPortNames();
            if (pLength.Length == 0) useController = false;

            if (useController)
            {
                foreach (string p in pLength)
                {
                    //Debug.Log("p: " + p);
                    port = p;
                }
                sp = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                OpenConnection();
                //WriteToArduino("PING");
                StartCoroutine(AsynchronousReadFromArduino((string s) => MovementData(s), () => Debug.LogError("err-sensor"), 10000f));
            }
        }
    }

    List<string> GetPortNames()
    {
        List<string> serialPorts = new List<string>();
        string[] ttys = Directory.GetFiles("/dev/", "tty.*");
        foreach (string dev in ttys)
        {
            // if (dev.StartsWith ("/dev/tty.*"))
            serialPorts.Add(dev);
        }

        string[] cus = Directory.GetFiles("/dev/", "cu.*");
        foreach (string dev in cus)
        {
            // if (dev.StartsWith ("/dev/cu.*"))
            serialPorts.Add(dev);
        }
        return serialPorts;
    }

    public void OpenConnection()
    {
        if (sp != null && !sp.IsOpen)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                Debug.Log("Closing port, because it was already open!");
            }
            else
            {
                try
                {
                    sp.Open();
                    sp.ReadTimeout = 50;  // sets timeout value before reporting error
                    Debug.Log("Port Opened: " + sp.PortName);
                    player.transform.localScale = player.transform.localScale * 2f;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString() + " port reopening problem?");
                }
            }
        }
        else
        {
            if (sp.IsOpen)
            {
                print("Port is already open");
            }
            else
            {
                print("Port == null");
            }
        }
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);
        string dataString = null;
        do
        {
            try
            {
                dataString = sp.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                try
                {
                    callback(obj: dataString);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    dataString = null;
                }
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.1f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void WriteToArduino(string message)
    {
        sp.WriteLine(message);
        sp.BaseStream.Flush();
    }

    void OnApplicationQuit()
    {
        if (useController) sp.Close();
    }

}
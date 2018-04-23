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

    private bool useController = true, gyroOnly = true, ballLoaded = true;
    private float timer = 20f;

    private CameraController view;
    private PlayerController paddle;
    private Rigidbody player;
    private Rigidbody ball;
    private Vector3 force;

    private int ax = 0 , ay = 0, gx = 0, gy = 0;
    private int moveX = 0, moveY = 0;
    private int turnX = 0, turnY = 0;
    private int axThreshold = 30, gxThreshold = 30, gxThresholdNeg = -30;
    private int gyThreshold = 75, gyThresholdNeg = -100;

    public float moveScale = 10f;
    public float moveGyroScale = 500f;
    public float powerScale = 10f;

    //private int axStart = 0, ayStart = 0;
    //private int gxStart = 0, gyStart = 0;
    //private int[] axA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //private int[] ayA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //private int[] gxA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //private int[] gyA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //private int counter = 0;
    //protected int countMax = 9;  //first 10 readings to calibrate, adjusted downward after calibration
    //protected int readInterval = 0; //number of readings to wait before averaging for movement

    void MovementData(string s)
    {
        s = s.Replace("a/g:\t", "");
        string[] arduinoData = s.Split('\t');

        if (arduinoData.Length == 4)    //6 if z
        {
            ax = int.Parse(arduinoData[0]);   //inverted in sketch
            ay = int.Parse(arduinoData[1]);   //see x/y marked on chip
            gx = int.Parse(arduinoData[3]);
            gy = int.Parse(arduinoData[4]);

            moveX = ax;
            moveY = ay;
            turnX = gx;
            turnY = gy;
            Debug.Log("accl (x:" + moveX + ",  y:" + moveY + ")\t gyro (x: " + turnX + ",  y:" + turnY + ")");
        }
        else
        {
            player.velocity = Vector3.zero;
            moveX = moveY = turnX = turnY = 0;
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
                force = new Vector3(moveScale * moveX, 0f, 0f);
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
                WriteToArduino("PING");
                StartCoroutine(AsynchronousReadFromArduino((string s) => MovementData(s), () => Debug.LogError("Error!"), 10000f));
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


//prior to calibration sketch
/*
    void MovementData(string s)
    {
        s = s.Replace("a/g:\t", "");
        string[] arduinoData = s.Split('\t');

        if (arduinoData.Length == 4)    //6 if z
        {
            ay = int.Parse(arduinoData[0]);   //inverted in sketch
            ax = int.Parse(arduinoData[1]);   //see x/y marked on chip
            //az = int.Parse(arduinoData[2]) / 100;
            gx = int.Parse(arduinoData[3]);
            gy = int.Parse(arduinoData[4]);
            //gz = int.Parse(arduinoData[5]) / 100; //z-axis mapped to 0 from arduino
            //Debug.Log("ax:" + ax + " ay:" + ay + " az:" + az + " gx:" + gx + " gy:" + gy + " gz:" + gz);

            axA[counter] = ax;
            ayA[counter] = ay;
            gxA[counter] = gx;
            gyA[counter] = gy;
            counter++;
        }

        if ((axStart == 0) && (ayStart == 0) && (counter == countMax)) //calibrate based on first 10 readings
        {
            countMax = readInterval + 1;
            counter = 0;
            axStart = (int)axA.Average();
            ayStart = (int)ayA.Average();
            gxStart = (int)gxA.Average();
            gyStart = (int)gyA.Average();

            Debug.Log("Calibration aX: " + axStart + " aY: " + ayStart + "\t gX: " + gxStart + " gY: " + gyStart);
            Array.Resize<int>(ref axA, countMax);
            Array.Resize<int>(ref ayA, countMax);
            Array.Resize<int>(ref gxA, countMax);
            Array.Resize<int>(ref gyA, countMax);
            player.transform.localScale = player.transform.localScale * 2f;
        }
        else if (counter == countMax)
        {
            moveX = (int)axA.Average() - axStart;
            moveY = (int)ayA.Average() - ayStart;
            turnX = (int)gxA.Average() - gxStart;
            turnY = (int)gyA.Average() - gyStart;

            player.velocity = Vector3.zero;
            Debug.Log("accl (x:" + moveX + ",  y:" + moveY + ")\t gyro (x: " + turnX + ",  y:" + turnY + ")");
        }
        else
        {
            moveX = moveY = turnX = turnY = 0;
        }

        counter = counter >= countMax ? 0 : counter;
        //Debug.Log(counter + " cm" + countMax + " ri" + readInterval);


        if (gyroOnly)
        {
            if (Mathf.Abs(turnX) > gxThreshold)
            {
                force = turnX > 0 ? new Vector3(moveGyroScale, 0f, 0f) : new Vector3(-1 * moveGyroScale, 0f, 0f);

                player.AddForce(force * 50f, ForceMode.VelocityChange);

                Debug.Log("gyro force: " + force.ToString() + "\tvelocity: " + player.velocity);
                //turnX = 0;
            }
        }
        else
        {
            if (Mathf.Abs(moveX) > axThreshold)
            {
                force = new Vector3(moveScale * moveX, 0f, 0f);

                player.AddForce(force, ForceMode.VelocityChange);

                Debug.Log("move force: " + force.ToString() + "\tvelocity: " + player.velocity);
                //moveX = 0;
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
 
 */

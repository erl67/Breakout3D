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

    private bool useController = true, gyroOnly = false;

    private Rigidbody player;
    private Rigidbody ball;

    private Vector3 force, prevForce;
    private int ax, ay, az;
    private int gx, gy, gz;
    private int axStart = 0, ayStart = 0;
    private int gxStart = 0, gyStart = 0;
    private int moveX = 0, moveY = 0;
    private int turnX = 0, turnY = 0;
    private int[] axA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] ayA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] gxA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] gyA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private int counter = 0;
    protected int countMax = 9;  //first 10 readings to calibrate, adjusted downward after calibration
    protected int readInterval = 0; //number of readings to wait before averaging for movement

    public float moveScale = 20f;
    public float bumpPower = 10f;

    void MovementData(string s)
    {
        s = s.Replace("a/g:\t", "");
        string[] arduinoData = s.Split('\t');

        if (arduinoData.Length == 6)
        {                  
            ay = int.Parse(arduinoData[0]) / 100;   //swapped from the sketch
            ax = int.Parse(arduinoData[1]) / 100;   //see x/y marked on chip
            az = int.Parse(arduinoData[2]) / 100;
            gx = int.Parse(arduinoData[3]) / 100;
            gy = int.Parse(arduinoData[4]) / 100;
            gz = int.Parse(arduinoData[5]) / 100;
            //Debug.Log("ax:" + ax + " ay:" + ay + " az:" + az + " gx:" + gx + " gy:" + gy + " gz:" + gz);

            axA[counter] = ax;
            ayA[counter] = ay;
            gxA[counter] = gx;
            gyA[counter] = gy;

            counter = counter > countMax ? 0 : counter++;
        }

        if ((axStart==0) && (ayStart==0) && (counter == countMax)) //calibrate based on first 10 readings
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
        }
        else if (counter == countMax)
        {
            moveX = (int) axA.Average() - axStart;
            moveY = (int) ayA.Average() - ayStart;
            turnX = (int) gxA.Average() - gxStart;
            turnY = (int) gyA.Average() - gyStart;
            Debug.Log("Motion  x:" + moveX + ",  y:" + moveY + "\t turn: " + turnX + ",  y:" + turnY);
        }
        else
        {
            moveX = moveY = turnX = turnY = 0;
        }

        if (!gyroOnly)
        {
            if (Mathf.Abs(moveX) > 20)
            {
                player.velocity = Vector3.zero;
                force = new Vector3(moveX, 0f, 0f) * moveScale;

                player.AddForce(force, ForceMode.Acceleration);

                Debug.Log("side force: " + force.ToString() + " velocity: " + player.velocity);

                moveX = 0;
            }
        } else
        {
            Debug.Log("gyro mode");
            player.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0f);

        }

        if (Mathf.Abs(turnY) > 30)   //increase hit power
        {
            player.mass = (int)(100 + turnY);
            Debug.Log("Power y:" + turnY + "  player.mass:" + player.mass);

            player.GetComponent<Renderer>().material.color = new Color(.5f, .5f, .5f);
            turnY = 0;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            gyroOnly = gyroOnly == false ? true : false;
            Debug.Log("changing mode  gyroOnly = " + gyroOnly);
        }
    }

    void Start()
    {
        player = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();
        //ball = GameObject.Find("Ball").gameObject.GetComponent<Rigidbody>(); //have have to wait on instance

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

                sp = new SerialPort(port, 38400, Parity.None, 8, StopBits.One);
                OpenConnection();
                //WriteToArduino("PING");

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
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                Debug.Log("Closing port, because it was already open!");
            }
            else
            {
                sp.Open();
                sp.ReadTimeout = 50;  // sets timeout value before reporting error
                Debug.Log("Port Opened: " + sp.PortName);
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

    /*
    void MoveObject(string[] arduinoData)
    {
        if (arduinoData.Length == 9)
        {
			 We need to calculate new position of the object based on acceleration.
			 * The data that comes in from the accelerometer is in meters per second per second (m/s^2)
			 * The equation is: s = ut + (1/2)a t^2
			 * where s is position, u is velocity at t=0, t is time and a is a constant acceleration.
			 * For example, if a car starts off stationary, and accelerates for two seconds with an 
			 * acceleration of 3m/s^2, it moves (1/2) * 3 * 2^2 = 6m
			 *

            float accX = float.Parse(arduinoData[0]) / 100; // Accelerometer X
            float accY = float.Parse(arduinoData[1]) / 100; // Accelerometer Y
            float accZ = float.Parse(arduinoData[2]) / 100; // Accelerometer Z

            float newAccX = transform.position.x + accX;
            float newAccY = transform.position.y + accY;
            float newAccZ = transform.position.z + accZ;
            transform.position = new Vector3(newAccX, newAccY, newAccZ);
            
			float gyroX = float.Parse (arduinoData [6]);
			float gyroY = float.Parse (arduinoData [8]);
			float gyroZ = float.Parse (arduinoData [9]);

			float newGyroX = transform.rotation.x + gyroX;
			float newGyroY = transform.rotation.y + gyroY;
			float newGyroZ = transform.rotation.z + gyroZ;		
        
            // transform.rotation = new Vector3(newGyroX, newGyroY, newGyroZ);
            // Quaternion target = Quaternion.Euler(newGyroX, newGyroY, newGyroZ);
            // transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        }
    }
*/

}

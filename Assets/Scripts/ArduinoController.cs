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

    private bool useController = true;

    private Rigidbody player;
    private Vector3 force, prevForce;
    private int ax, ay, az;
    private int axDiff, ayDiff, azDiff;
    private int gx, gy, gz;

    public int[] axA = {0,0,0,0,0};
    public int[] ayA = {0,0,0,0,0};

    private int xStart = 0, yStart = 0;
    private int moveX = 0, moveY = 0;

    private int counter = 0;

    void MovementData(string s)
    {

        s = s.Replace("a/g:\t", "");
        string[] arduinoData = s.Split('\t');

        if (arduinoData.Length == 6)
        {                  
            ay = int.Parse(arduinoData[0]) / 100;   //swapped from the sketch
            ax = int.Parse(arduinoData[1]) / 100;   //see x/y marked on chip
            az = int.Parse(arduinoData[2]) / 100;
            //gy = int.Parse(arduinoData[3]) / 100;
            //gx = int.Parse(arduinoData[4]) / 100;
            //gz = int.Parse(arduinoData[5]) / 100;
        }

        axA[counter] = ax;
        ayA[counter] = ay;

        counter++;

        //xStart = ((xStart == 0) && (counter == 5)) ? (int)axA.Average() : xStart;

        if ((xStart==0) && (yStart==0) && (counter == 5))
        {
            xStart = (int)axA.Average();
            yStart = (int)ayA.Average();
            Debug.Log("\tstartX: " + xStart + " startY: " + yStart);
        }
        else if (counter == 5)
        {
            moveX = (int) axA.Average() - xStart;
            moveY = (int) ayA.Average() - yStart;
        }

        if (Mathf.Abs(moveX) > 5)
        {
            Debug.Log("\tmove  tx:" + moveX + " y:" + moveY);
        }

        counter = counter >= 5 ? 0 : counter;

        //Debug.Log(" c: " + counter + " axA: " + axA[counter] + " avg: " + axA.Average());

        //Debug.Log(player.velocity + "\t" + ax + " " + ay + " " + az + " " + gx + " " + gy + " " + gz);

        if (Mathf.Abs(moveX) > 10) {
            force = new Vector3(moveX, 0f, 0f) *5f;
            player.AddForce(force, ForceMode.Acceleration);
            prevForce = force; //not sure why I saved this
            Debug.Log("force:\t"+force.ToString());
            moveX = 0;
            moveY = 0;
        }
    }

    void Start()
    {
        player = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();

        if (useController)
        {
            string port = null;
            var pLength = System.IO.Ports.SerialPort.GetPortNames();
            if (pLength.Length == 0) useController = false;

            if (useController)
            {
                foreach (string p in pLength)
                {
                    Debug.Log("p: " + p);
                    port = p;
                }

                sp = new SerialPort(port, 38400, Parity.None, 8, StopBits.One);
                OpenConnection();
                //WriteToArduino("PING");

                //StartCoroutine(AsynchronousReadFromArduino((string s) => Debug.Log(s), () => Debug.LogError("Error!"), 10000f));
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

    //Function connecting to Arduino
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
                sp.Open();  // opens the connection
                sp.ReadTimeout = 50;  // sets the timeout value before reporting error
                Debug.Log("Port Opened!");
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

    /*
    void Update()
    {
        if (sp != null)
        {
            try
            {
                //Read incoming data
                strIn = sp.ReadLine();
                if (!string.IsNullOrEmpty(strIn))
                {
                    // Split string into an array
                    string[] arduinoData = strIn.Split(',');
                    MoveObject(arduinoData);
                }
            }
            catch (Exception ex)
            {
                // Do nothing - just ignore	
            }
        }
    }
 
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
 

    public void WriteToArduino(string message)
    {
        sp.WriteLine(message);
        sp.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        sp.ReadTimeout = timeout;
        try
        {
            return sp.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }
*/
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

            //Debug.Log(dataString);

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

    void OnApplicationQuit()
    {
        if (useController) sp.Close();       
    }

}

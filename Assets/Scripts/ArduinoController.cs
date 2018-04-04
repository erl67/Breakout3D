using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System;

public class ArduinoController : MonoBehaviour
{
    // SerialPortConnectorMac portConnect;
    public static SerialPort sp;
    public static string strIn;
    public static List<string> portList;
    public float smooth = 2.0F;
    private Vector3 prevPosition;

    private bool useController = true;

    void Start()
    {
        if (useController)
        {
            string port = null;

            //Debug.Log(System.IO.Ports.SerialPort.GetPortNames().Length);
            foreach (string p in System.IO.Ports.SerialPort.GetPortNames())
            {
                Debug.Log("p: " + p);
                port = p;
            }

            //portList = GetPortNames();
            //foreach (string port in portList)
            //{
            //    Debug.Log("port: " + port);
            //}
            //Debug.Log(portList);

            sp = new SerialPort(port, 38400, Parity.None, 8, StopBits.One);
            OpenConnection();
            WriteToArduino("PING");
            StartCoroutine(AsynchronousReadFromArduino((string s) => Debug.Log(s), () => Debug.LogError("Error!"), 10000f));
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

    // Update is called once per frame
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

    void OnApplicationQuit()
    {
        if (useController)
        {
            sp.Close();
        }
    }

    void MoveObject(string[] arduinoData)
    {
        if (arduinoData.Length == 9)
        {

            /*
			 * We need to calculate new position of the object based on acceleration.
			 * The data that comes in from the accelerometer is in meters per second per second (m/s^2)
			 * The equation is: s = ut + (1/2)a t^2
			 * where s is position, u is velocity at t=0, t is time and a is a constant acceleration.
			 * For example, if a car starts off stationary, and accelerates for two seconds with an 
			 * acceleration of 3m/s^2, it moves (1/2) * 3 * 2^2 = 6m
			 */

            float accX = float.Parse(arduinoData[0]) / 100; // Accelerometer X
            float accY = float.Parse(arduinoData[1]) / 100; // Accelerometer Y
            float accZ = float.Parse(arduinoData[2]) / 100; // Accelerometer Z


            float newAccX = transform.position.x + accX;
            float newAccY = transform.position.y + accY;
            float newAccZ = transform.position.z + accZ;
            transform.position = new Vector3(newAccX, newAccY, newAccZ);

            /*
			float gyroX = float.Parse (arduinoData [6]);
			float gyroY = float.Parse (arduinoData [8]);
			float gyroZ = float.Parse (arduinoData [9]);

			float newGyroX = transform.rotation.x + gyroX;
			float newGyroY = transform.rotation.y + gyroY;
			float newGyroZ = transform.rotation.z + gyroZ;
			*/

        
            // transform.rotation = new Vector3(newGyroX, newGyroY, newGyroZ);
            // Quaternion target = Quaternion.Euler(newGyroX, newGyroY, newGyroZ);
            // transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        }
    }

    public bool IsNumber(string s)
    {
        bool value = true;
        foreach (char c in s.ToCharArray())
        {
            value = value && char.IsDigit(c);
        }
        return value;
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
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
}

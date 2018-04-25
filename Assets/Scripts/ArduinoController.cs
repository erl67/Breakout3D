using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System;
using System.Threading;

public class ArduinoController : MonoBehaviour
{
   // SerialPortConnectorMac portConnect;
   public static SerialPort sp;
   public static string strIn;
   public static List<string> portList;
   public float smooth = 2.0F;
   private Vector3 prevPosition;

   private bool useController = true, gyroOnly = true, ballLoaded = true;
   private float timer = 6f;

   private CameraController view;
   private PlayerController paddle;
   private Rigidbody player;
   private Rigidbody ball;
   private Vector3 force;

   private int moveX = 0, moveY = 0;
   private int turnX = 0, turnY = 0;

   private int axThreshold = 40; //move
   private int ayThreshold = 50; //unused
   private int gxThreshold = 35; //tilt move
   private int gyThreshold = 70; //launch ball 
   private int gyThresholdNeg = -30; //resetCamera/life

   public float moveAccScale = 200f;
   public float moveGyroScale = 500f;
   //public float powerScale = 10f;

   void MovementData(string s)
   {
       if (s.Substring(0, 4) == "a/g:")
       {
           s = s.Replace("a/g:\t", "");
           string[] arduinoData = s.Split('\t');

           if (arduinoData.Length == 4)
           {
               moveX = int.Parse(arduinoData[0]);   //inverted, x/y swapped in sketch
               moveY = int.Parse(arduinoData[1]);
               turnX = int.Parse(arduinoData[2]);
               turnY = int.Parse(arduinoData[3]);

               Debug.Log("gyro (x: " + turnX + ",  y:" + turnY + ")\tblocks:" + GameObject.FindGameObjectsWithTag("block").Length);

               //Debug.Log("accl (x:" + moveX + ",  y:" + moveY + ")\t gyro (x: " + turnX + ",  y:" + turnY + ")");
           }
           else
           {
               moveX = moveY = turnX = turnY = 0;
               player.velocity = Vector3.zero;
           }

           if (gyroOnly)
           {
               if (Mathf.Abs(turnX) > gxThreshold)
               {
                   force = new Vector3(turnX, 0f, 0f) * moveGyroScale;
                   player.AddForce(force, ForceMode.Impulse);
                   Debug.Log("gyro force: " + force.ToString() + "\tvelocity: " + player.velocity);
               }
           }
           else
           {
               if (Mathf.Abs(moveX) > axThreshold)
               {
                   force = new Vector3(moveX, 0f, 0f) * moveAccScale;
                   player.AddForce(force, ForceMode.Impulse);
                   Debug.Log("move force: " + force.ToString() + "\tvelocity: " + player.velocity);
               }
           }

           if (turnY > gyThreshold && ballLoaded)   //launch ball
           {
               ballLoaded = false;
               paddle.LaunchBall();
               //player.mass = (int)(100 + turnY);
           }
           else if (turnY < gyThresholdNeg)
           {
               view.ResetCamera();
           }

           if (Time.timeScale == 0 && (turnY < (gyThresholdNeg / 1.5f)))
           {
               Debug.Log("\tStarting new life");
               ballLoaded = true;
               paddle.StopStartBox();
               Time.timeScale = 1;
           }
       }
       //else Debug.Log("data-> " + data);            
   }

   void FixedUpdate()
   {
       if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
       {
           gyroOnly = !gyroOnly;
           player.transform.localScale = gyroOnly ? player.transform.localScale * .75f : player.transform.localScale * 1.25f;
           Debug.Log("changing mode  gyroOnly = " + gyroOnly);
       }

       if (GameObject.FindGameObjectsWithTag("ball").Length < 1 || timer < Time.time)
       {
           ballLoaded = true;
           timer = Time.time + 5f;
       }
   }

   void Start()
   {
       StopAllCoroutines(); //stop ReadFromArduino
       //StopCoroutine(AsynchronousReadFromArduino(null, null, 0f));
       //yield return null WaitForSeconds(1);
       player = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();
       view = (CameraController)GameObject.Find("MainCamera").GetComponent("CameraController");
       paddle = (PlayerController)GameObject.Find("Player").GetComponent("PlayerController");

       //if (!sp.IsOpen) Connect();
       try
       {
           Connect();
       }
       catch (Exception e)
       {
           Debug.Log(e.ToString() + " port reopening problem?");
       }
   }

   private void Connect()
   {
       string port = null;
       var pLength = System.IO.Ports.SerialPort.GetPortNames();
       //if (pLength.Length == 0) useController = false;

       foreach (string p in pLength)
       {
           //Debug.Log("p: " + p);
           port = p;
       }
       sp = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
       OpenConnection();

       try
       {
           StartCoroutine(AsynchronousReadFromArduino((string s) => MovementData(s), () => Debug.LogError("err-sensor"), 10000f));
       }
       catch (Exception e)
       {
           Debug.Log(e.ToString() + " error starting coroutine");
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
                   player.transform.localScale = player.transform.localScale * 1.5f;
               }
               catch (Exception e)
               {
                   Debug.Log(e.ToString() + " port reopening problem?");
               }
           }
       }
       else
       {
           if (sp.IsOpen) print("Port is already open");
           else print("Port == null");
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
           else yield return new WaitForSeconds(0.1f);

           nowTime = DateTime.Now;
           diff = nowTime - initialTime;

       } while (diff.Milliseconds < timeout);

       if (fail != null) fail();
       yield return null;
   }

   void OnApplicationQuit()
   {
       //if (useController) sp.Close();
   }

}

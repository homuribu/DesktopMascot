using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO.Ports;
using UnityEngine;
using UniRx;

public class ESP32BTCommunicator : MonoBehaviour
{

    public string portName;
    public int baurate;

    SerialPort serial;
    bool isLoop = true;

    void Start()
    {
        this.serial = new SerialPort(portName, baurate, Parity.None, 8, StopBits.One);

        try
        {
            this.serial.Open();
            Scheduler.ThreadPool.Schedule(() => ReadData()).AddTo(this);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("can not open serial port");
        }
    }

    public void ReadData()
    {
        while (this.isLoop)
        {
            string message = this.serial.ReadLine();
            string[] datas = message.Split(',');
            if (datas[0] == "A")//airdata
            {


            }
            else
            {

            }
            Debug.Log(message);
        }
    }


    void OnDestroy()
    {
        this.isLoop = false;
        this.serial.Close();
    }
}
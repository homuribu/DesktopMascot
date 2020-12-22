using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kirurobo;
using UniRx;
using System;
using System.Text.Json;

public class MenuSystem : MonoBehaviour
{

    private UniWindowController uniwinc = null;

    [Serializable]
    public class AirData
    {
        /*
        public double temp { get; set; }
        public double press  { get; set; }
        public double hum { get; set;}
        */
        public double temp;
        public double press;
        public double hum;
    }

    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string description;
    }
    [SerializeField] private CanvasGroup canvasGroup;
    private SocketClient socketClient;
    // Start is called before the first frame update
    void Start()
    {
        socketClient = new SocketClient();
        uniwinc = GameObject.FindObjectOfType<UniWindowController>();
        Node root = new MenuNode();
        Node setting = new Node("Setting", 1);
        root.AddChild(setting);
        Node set_transparent = new Node("透過切り替え", 1);
        set_transparent.AddCallback(() =>
        {
            uniwinc.isTransparent = !uniwinc.isTransparent;
        });
        setting.AddChild(set_transparent);

        bool on = uniwinc.isTopmost;
        Node set_topmost = new Node("Topmost:ON ", 1);

        setting.AddCallback(() =>
        {
            on = uniwinc.isTopmost;
            set_topmost.Text = on ? "Topmost:ON" : "Topmost:OFF";

        });
        set_topmost.AddCallback(() =>
        {
            on = !on;
            set_topmost.Text = on ? "Topmost:ON" : "Topmost:OFF";
            uniwinc.isTopmost = on;
        });

        setting.AddChild(set_topmost);

        Node set_deactivate = new Node("半透明:OFF", 1);
        bool on2 = false;
        set_deactivate.AddCallback(() =>
        {
            on2 = !on2;
            canvasGroup.alpha = on2 ? 0.5f : 1.0f;
            set_deactivate.Text = on2 ? "半透明:ON" : "半透明:OFF";

        });
        setting.AddChild(set_deactivate);

        Node sensor = new Node("Sensor", 1);
        ESP32Communicator e32c = new ESP32Communicator();
        root.AddChild(sensor);
        Node temperature = new Node("--℃", 1);
        Node humidity = new Node("--％", 1);
        Node pressure = new Node("----Pa", 1);

        sensor.AddChild(humidity);
        sensor.AddChild(temperature);
        sensor.AddChild(pressure);
        //ESP32Communicator e32c = new ESP32Communicator();

        sensor.AddCallback(() =>
        {
            /*
            var _airdata = e32c.GetAirData();
            _airdata.Subscribe(_data =>
            {
                AirData data = JsonUtility.FromJson<AirData>(_data);
                temperature.Text = data.temp+"℃";
                humidity.Text = data.hum +"％";
                pressure.Text = data.press+ "Pa";
            });
            */
            var _airdata = e32c.GetAirData();
            _airdata.Subscribe(_data =>
            {
                AirData data = JsonUtility.FromJson<AirData>(_data);
                temperature.Text = data.temp + "℃";
                humidity.Text = data.hum + "％";
                pressure.Text = data.press + "Pa";
            });

        });

        //Node test2 = new Node("<quad material=1 size=20 x=0.1 y=0.1 width=0.5 height=0.5>", 1);
        Node clock = new Node("Clock", 1);
        root.AddChild(clock);

        Node clock_display = new Node("--:--", 1);
        clock.AddChild(clock_display);

        IDisposable clock_update = null;
        clock.AddCallback(() =>
        {
            clock_update = Observable.Interval(TimeSpan.FromSeconds(0.1)).Subscribe(_ =>
            {
                if (clock_display.speechBubble == null)
                {
                    clock_update.Dispose();
                    return;
                }
                //clock_display.Text = DateTime.Now.ToShortTimeString();
                clock_display.Text = DateTime.Now.ToLongTimeString();
            }).AddTo(this);
        });

        Node tool = new Node("Tool", 1);
        root.AddChild(tool);
        Node bluetooth = new Node("Bluetooth\n ON");
        tool.AddChild(bluetooth);

        Node socketClientTest = new Node("SocketClient\nTest");
        socketClientTest.AddCallback(() =>
        {
            socketClient.ConnentionTest();

        });
        tool.AddChild(socketClientTest);

        root.GenerateBubble(2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

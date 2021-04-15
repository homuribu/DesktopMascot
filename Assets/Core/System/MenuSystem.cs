using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kirurobo;
using UniRx;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Core.Communication;
using Core.UI;
using UnityEditor;

namespace Core.System
{
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
        private SocketServer socketServer;
        private SpeechBubbleMaker maker;
    
        // Start is called before the first frame update
        void Start()
        {
            socketClient = new SocketClient();
            socketServer = GameObject.FindObjectOfType<SocketServer>();
            maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
            uniwinc = GameObject.FindObjectOfType<UniWindowController>();
            Node root = new MenuNode();
            Node setting = new Node("せってい", 1);
            root.AddChild(setting);
            var setTransparent = new SwitchNode("背景透過中だよ","背景透過してないよ",  1,uniwinc.isTransparent );
            setting.AddCallback(() =>
            {
                setTransparent.ON = uniwinc.isTransparent;
            });
            setTransparent.AddCallback(() =>
            {
                uniwinc.isTransparent = setTransparent.ON;
            });
            setting.AddChild(setTransparent);

            var setTopmost = new SwitchNode("最前面固定するよ","最前面固定しないよ", 1,uniwinc.isTopmost );
            setting.AddCallback(() =>
            {
                setTopmost.ON = uniwinc.isTopmost;
            });
            setTopmost.AddCallback(() =>
            {
                uniwinc.isTopmost = setTopmost.ON;
            });
            setting.AddChild(setTopmost);

            var setTranslucent = new SwitchNode("半透明だよ","半透明じゃないよ", 1, false);
            setTranslucent.AddCallback(() =>
            {
                canvasGroup.alpha = setTranslucent.ON ? 0.5f : 1.0f;
            });
            setting.AddChild(setTranslucent);
        
            var setRandspeak = new SwitchNode("話すよー","黙ってるよ...",  + 1);
            setRandspeak.AddCallback(() =>
            {
                if (setRandspeak.ON)
                {
                    maker.StopSpeak();
                }
                else
                {
                    maker.StartSpeak();
                }
            });
            setting.AddChild(setRandspeak);
            Node remocon = new Node("リモコン", 1);
            // root.AddChild(remocon);
            Node right_off = new Node("明かりを消す?", 1);
            right_off.AddCallback(() =>
            {
                HttpCommunicator hc = new HttpCommunicator();
                var url = "https://api.switch-bot.com/v1.0/devices/01-202101071744-43895891/commands";
                // hc.GetHttpAsync()
            });
            
            
            

            Node sensor = new Node("Sensor", 1);
            // root.AddChild(sensor);
            PythonExecutor pyexe = GameObject.FindObjectOfType<PythonExecutor>();
        
            socketServer.AddCommand("speak", mes =>
            {
                maker.GenerateFromThread(1, mes, 3, 1);
            });
        
            socketServer.AddCommand("long_speak", mes =>
            {
                maker.GenerateFromThread(1, mes, -1, 1);
            });
        
            socketServer.AddCommand("echo", mes =>
            {
                pyexe.SendCmd("nothing");
            });

            Node sensor_mes = new Node("Lauching...");
            sensor.AddChild(sensor_mes);
            pyexe.AddCommand("log", (mes) =>
            {
                Debug.Log("[python][log] " + mes);
            });

            sensor.AddCallback(() =>
            {
                sensor_mes.Text = "Lauching...";
                pyexe.ExecuteOnThread(); //実行
                Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                {
                    pyexe.p_WriteInputData("echo Message from C#");
                });
            });
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
            //Node bluetooth = new Node("Bluetooth\n ON");
            //tool.AddChild(bluetooth);
            Node pyexecutor_interface = new Node("pyexecutor");
            tool.AddChild(pyexecutor_interface);

            Node gui_start = new Node("GUIStart");
            pyexecutor_interface.AddChild(gui_start);

            pyexe.AddCommand("launched", (mes) =>
            {
                gui_start.Text = "GUI Launched";
            });

            gui_start.AddCallback(() =>
            {
                gui_start.Text = "GUI Started";
                pyexe.SendCmd("gui_start");

            });


            Node socketClientTest = new Node("SocketClient\nTest");
            socketClientTest.AddCallback(() =>
            {
                Task.Run(socketClient.ConnentionTest);

            });
            tool.AddChild(socketClientTest);
            
            Node shutdown = new Node("Shutdown");
            shutdown.AddCallback(() =>
            {
                #if UNITY_EDITOR
                  UnityEditor.EditorApplication.isPlaying = false;
                #elif UNITY_STANDALONE
                  UnityEngine.Application.Quit();
                #endif
            });
            tool.AddChild(shutdown);
            Node speaktest = new Node("speaktest");
            tool.AddChild(speaktest);
            Node title1 = new Node("精密計測工学2\n レポート");
            speaktest.AddChild(title1);
            // Node title2 = new Node("精密計測工学2");
            // speaktest.AddChild(title2);

            root.GenerateBubble(2);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
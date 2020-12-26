using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
//using UniRx.Async;
using System.Threading.Tasks;

namespace Core.System
{
    public class PythonExecutor : MonoBehaviour
    {
        public String FileName { get; set; }
        public String WorkingDirectory { get; set; }
        public String Arguments { get; set; }
        public String InputString { get; set; }
        public String StandardOutput { get; set; }
        public int ExitCode { get; set; }

        private StringBuilder standardOutputStringBuilder = new StringBuilder();

        private global::System.Diagnostics.Process p;


        protected Dictionary<string, Action<string>> callbacks = new Dictionary<string, Action<string>>();

        public PythonExecutor()
        {

        }
        private void Start()
        {
            FileName = @"C:\Users\iesho\PycharmProjects\pyUnityCommunicator\venv\Scripts\python.exe";
            WorkingDirectory = @"C:\Users\iesho\PycharmProjects\pyUnityCommunicator";
            Arguments = @"C:\Users\iesho\PycharmProjects\pyUnityCommunicator\main.py";
            InputString = ""; //標準入力に与えるデータ
            AddCommand("err", msg =>
            {
                Debug.LogError("[python][err] " + msg);

            });
            ExecuteOnThread();
        }

        public void ExecuteOnThread()
        {
            Task.Run(() =>
            {
                Execute();
            });

        }
        /// 実行ボタンクリック時の動作
        public void Execute()
        {

            global::System.Diagnostics.ProcessStartInfo psInfo = new global::System.Diagnostics.ProcessStartInfo();
            psInfo.FileName = this.FileName;
            psInfo.WorkingDirectory = this.WorkingDirectory;
            psInfo.Arguments = this.Arguments;

            psInfo.CreateNoWindow = true;
            psInfo.UseShellExecute = false;
            psInfo.RedirectStandardInput = true;
            psInfo.RedirectStandardOutput = true;
            psInfo.RedirectStandardError = true;

            // Process p = Process.Start(psInfo);
            p = new global::System.Diagnostics.Process();
            p.StartInfo = psInfo;
            p.OutputDataReceived += p_OutputDataReceived;
            p.ErrorDataReceived += p_ErrorDataReceived;

            // プロセスの実行
            p.Start();

            // 標準入力への書き込み
            //if (InputString.Length > 0)
            p_WriteInputData(InputString);

            //非同期で出力とエラーの読み取りを開始
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            // 終わるまでまつ
            p.WaitForExit();
            this.ExitCode = p.ExitCode;
            this.StandardOutput = standardOutputStringBuilder.ToString();
        }

        /// <summary>
        /// 標準出力データを受け取った時の処理
        /// </summary>
        public void AddCommand(string cmd, Action<string> _callback)
        {
            callbacks.Add(cmd, _callback);

        }

        public void SendCmd(string cmd, string msg = "")
        {
            p_WriteInputData(cmd + " " + msg);
        }

        public void p_WriteInputData(string inputstr)
        {
            p.StandardInput.WriteLine(inputstr);
            //using (StreamWriter sw = p.StandardInput)
            //{
            //sw.Write(inputstr);
            //}
        }

        /// <summary>
        /// 標準出力データを受け取った時の処理
        /// </summary>
        public void p_OutputDataReceived(object sender,
            global::System.Diagnostics.DataReceivedEventArgs e)
        {
            //processMessage(sender, e);
            if (e != null && e.Data != null && e.Data.Length > 0)
            {
                //standardOutputStringBuilder.Append(e.Data + "\n");
                //Debug.Log(e.Data);
                var temp = e.Data.Split(' ');
                var cmd = temp[0];
                var mes = "";
                if (temp.Length > 1)
                {
                    int count = 0;
                    foreach (string m in temp)
                    {
                        if (count > 0)
                            mes += m + " ";
                        count++;
                    }
                }
                if (callbacks.ContainsKey(cmd))
                {
                    callbacks[cmd](mes);
                }
                else
                {
                    Debug.Log("Command not found:" + e.Data);
                }
            }
        }

        /// <summary>
        /// 標準エラーを受け取った時の処理
        /// </summary>
        public void p_ErrorDataReceived(object sender,
            global::System.Diagnostics.DataReceivedEventArgs e)
        {
            //必要な処理を書く
            if (e != null && e.Data != null && e.Data.Length > 0)
            {
                //standardOutputStringBuilder.Append(e.Data + "\n");
                //var bytes = System.Text.Encoding.GetEncoding(932).GetBytes(e.Data);
                //var mes = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.LogError(e.Data);
            }
        }

        private void OnApplicationQuit()
        {
            p.Kill();
        }
    }
}
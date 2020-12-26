using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SocketServer : MonoBehaviour
{
    //自分自身を指すIPアドレス
    public string mIpAddress = "127.0.0.1";
    //ポート番号は適当　ただしクライアントと合わせること
    public int mPortNumber = 9998;

    protected Dictionary<string, Action<string>> callbacks = new Dictionary<string, Action<string>>();
    
    private TcpListener mListener;
    private TcpClient mClient;

    // ソケット接続準備、待機
    public void Start()
    {
        var ip = IPAddress.Parse(mIpAddress);
        mListener = new TcpListener(ip, mPortNumber);
        mListener.Start();
        //コールバック設定　第二引数はコールバック関数に渡される
        mListener.BeginAcceptSocket(DoAcceptTcpClientCallback, mListener);
    }

    // クライアントからの接続処理
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        //渡されたものを取り出す
        var listener = (TcpListener)ar.AsyncState;
        mClient = listener.EndAcceptTcpClient(ar);
        print("Connect: " + mClient.Client.RemoteEndPoint);

        // 接続した人とのネットワークストリームを取得
        var stream = mClient.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);

        // 接続が切れるまで送受信を繰り返す
        while (mClient.Connected)
        {
            while (!reader.EndOfStream)
            {
                // 一行分の文字列を受け取る
                var mes = reader.ReadLine();
                var temp =     mes.Split(' ');
                // var temp = e.Data.Split(' ');
                var cmd = temp[0];
                var args = "";
                if (temp.Length > 1)
                {
                    int count = 0;
                    foreach (string m in temp)
                    {
                        if (count > 0)
                            args += m + " ";
                        count++;
                    }
                }
                if (callbacks.ContainsKey(cmd))
                { 
                    Task.Run(() =>
                    {
                        callbacks[cmd](args);
                    });
                }
                else
                {
                    Debug.Log("Command not found:" + mes);
                }
            }

            // クライアントの接続が切れたら
            if (mClient.Client.Poll(1000, SelectMode.SelectRead) && (mClient.Client.Available == 0))
            {
                Debug.Log("Disconnect: " + mClient.Client.RemoteEndPoint);
                mClient.Close();
                break;
            }
        }
    }
    
    public void AddCommand(string cmd, Action<string> _callback)
    {
        callbacks.Add(cmd, _callback);

    }

    // 終了処理
    protected virtual void OnApplicationQuit()
    {
        if (mListener != null) mListener.Stop();
        if (mClient != null) mClient.Close();
    }
}
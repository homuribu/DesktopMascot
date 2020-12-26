using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UniRx;
using System.Text.Json;

public class ESP32Communicator
{
    private const string BaseURL = "http://192.168.100.180";

    void Update()
    {

        //StartCoroutine("GetTemperatureImpl",BaseURL+"/temperature");
    }

    // field: temp, press, hum
    //public IObservable<Dictionary<string,string>> GetAirData()
    public IObservable<string> GetAirData()
    {
        return GetHttpAsync(BaseURL + "/airdata");
            /*
            .Select(jsonstr =>
            {
                //Debug.Log(jsonstr);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstr);
            });
            */
    }
    public IObservable<string> GetHumidity()
    {
        return GetHttpAsync(BaseURL + "/humidity");
    }

    public IObservable<string> GetTemperature()
    {
        return GetHttpAsync(BaseURL + "/temperature");
    }


    public IObservable<string> GetHttpAsync(string url)
    {
        return Observable.FromCoroutine<string>(obj => GetHTTP(obj, url));
    }
    
    //public IEnumerator GetTemperatureImpl(string url, string result)
    IEnumerator GetHTTP(IObserver<string> observer, string url)
    {

        //URLをGETで用意
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        //URLに接続して結果が戻ってくるまで待機
        yield return webRequest.SendWebRequest();

        //エラーが出ていないかチェック
        if (webRequest.isNetworkError)
        {
            //通信失敗
            Debug.Log(webRequest.error);
            observer.OnError(new Exception(webRequest.error));
        }
        else
        {
            observer.OnNext(webRequest.downloadHandler.text);
            observer.OnCompleted();
            //通信成功
            //result = webRequest.downloadHandler.text;
        }
    }
}

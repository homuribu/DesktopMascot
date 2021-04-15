using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UniRx;
using System.Text.Json;

namespace Core.Communication
{
    public class HttpCommunicator 
    {
        public IObservable<string> GetAsync(string url)
        {
            return Observable.FromCoroutine<string>(obj => Get(obj, url));
        }
        
        public IObservable<string> PostAsync(string url)
        {
            return Observable.FromCoroutine<string>(obj => Get(obj, url));
        }
        
        IEnumerator Get(IObserver<string> observer, string url)
        {
            //URLをGETで用意
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            return HttpRequest(observer, webRequest);
        }

        
        public IObservable<string> HttpRequestAsync(UnityWebRequest webRequest)
        {
            return Observable.FromCoroutine<string>(obj => HttpRequest(obj, webRequest));
        }
        
        IEnumerator HttpRequest(IObserver<string> observer, UnityWebRequest webRequest)
        {
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UniRx;
using Kirurobo;
public class SpeechBubbleMaker : MonoBehaviour
{
    GameObject speechBubble = null;
    List<List<float>> bubblePlace = new List<List<float>>();
    int strIndex = 0;
    int count = 0;
    bool randspeachactive = true;
    bool pause = false;
    IDisposable randspeach = null;


    void Start()
    {
        bubblePlace.Add(new List<float>{-87, -330, 0, 1, -1});
        bubblePlace.Add(new List<float>{-100, -130, 0, 0, 2});
        bubblePlace.Add(new List<float>{-91, 75, 0, 0, -1});
        bubblePlace.Add(new List<float>{180, 100, 1, 0, 2});
        
        speechBubble = Resources.Load("SpeechBubble") as GameObject;
        if (speechBubble== null)
        {
            Debug.Log("model null");
            return;
        }

        StartSpeak();

    }

    public SpeechBubble Generate(int index, string _text, float time = 3,int mode = 0)
    {
        return Generate(bubblePlace[index][0], bubblePlace[index][1], _text,time, bubblePlace[index][4] == -1?mode:(int)bubblePlace[index][4], bubblePlace[index][2] == 1, bubblePlace[index][3]==1);
    }
    public SpeechBubble Generate(float pos_x, float pos_y,  string _text, float time = 3,int mode = 0, bool reverseX=false, bool reverseY = false)
    {
        var ui_clone = (Instantiate(speechBubble, new Vector3(pos_x, pos_y, 0), Quaternion.identity)as GameObject).GetComponent<SpeechBubble>();
        ui_clone.Init();
        ui_clone.transform.SetParent(GameObject.Find("DragMoveCanvas").transform, false);
        ui_clone.SetMode(mode, reverseX, reverseY);
        ui_clone.Speak(_text, time);
        return ui_clone;
    }
    public void BreakAllBubbles()
    {
        var bubbles = GameObject.FindObjectsOfType<SpeechBubble>();
        var menubabble = GameObject.Find("MenuBubble").GetComponent<SpeechBubble>();
        foreach(var _b in bubbles)
        {
            if (menubabble == _b)
            {
                continue;
            }
            _b.Break();
        }
    }
    public void StopSpeak()
    {
        randspeachactive = false;
        BreakAllBubbles();
    }
    public void PauseSpeak(bool _pause)
    {
        pause = _pause;
        if (pause)
        {
            BreakAllBubbles();
        }
    }
    public void StartSpeak()
    {
        randspeachactive = true;
        //RandomSpeak();
        if(randspeach == null)
        {
            randspeach = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ =>
            {
                if (!randspeachactive)
                {
                    //自殺

                    randspeach?.Dispose();
                    randspeach = null;
                    return;
                }
                if (!pause)
                {
                    RandomSpeak();
                }

            }).AddTo(this);
        }

    }
    public void RandomSpeak()
    {
            var _rand = UnityEngine.Random.Range(0, 2);

            RandomGenerate();
            return;

            if(_rand == 0)
            {
                RandomGenerate();
            }
            else
            {
                RandomGenerate2();
            }

    }
    void RandomGenerate2()
    {
        //var strList = new List<string>() { "おなかすいたなぁ", "がんばってるなぁ", "んーっ", "にゃー" };
        var strList = new List<string>() { "かわいい思考" };
        var _index = UnityEngine.Random.Range(0, strList.Count);
        while(strIndex == _index)
        {
            _index = UnityEngine.Random.Range(0, strList.Count);
        }
        strIndex = _index;
        Generate(count%bubblePlace.Count, strList[strIndex], 3, 1);
        count += 1;

    }
    void RandomGenerate()
    {
        var strList = new List<string>(){ "おはよう！", "元気?", "やっほー", "おなかすいたー", "がんばれー", "ご飯食べた?", "ねーねー"};
        var _index = UnityEngine.Random.Range(0, strList.Count);
        while(strIndex == _index)
        {
            _index = UnityEngine.Random.Range(0, strList.Count);
        }
        strIndex = _index;
        Generate(count%bubblePlace.Count, strList[strIndex], 3, 0);
        count += 1;
    }
}

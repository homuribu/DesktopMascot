using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuNode : Node
{
    public MenuNode():base("Menu", 0)
    {
    }
    bool menu_called = false;
    //このNodeをクリックした時点で呼ばれるもの
    public override void  OnClick()
    {
        menu_called = !menu_called;
        if (menu_called)
        {
            BreakAllBubbles();
            foreach(Action _func in callback)
            {
                _func();
            }
            //parent?.BreakChildren();

            var maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
            maker.StopSpeak();
            GenerateChildrenBubble();
        }
        else
        {
            BreakAllBubbles();
            var maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
            maker.StartSpeak();

        }
    }
    public override void OnBreak()
    {

    }
    public override void BreakBubble()
    {
        speechBubble.Break();
    }
    //TODO 本当にジェネレートするようにする
    public override void GenerateBubble(int num = 2)
    {
        speechBubble = GameObject.Find("MenuBubble").GetComponent<SpeechBubble>();
        speechBubble.SetNode(this);
    }

    //クソ実装、イベントを勉強する気になったら書き換える
    public void BreakAllBubbles()
    {
        var bubbles = GameObject.FindObjectsOfType<SpeechBubble>();
        foreach(var _b in bubbles)
        {
            if (speechBubble == _b)
            {
                continue;
            }
            _b.Break();
        }
    }
}

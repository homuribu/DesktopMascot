using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuNode : Node
{
    SpeechBubbleMaker maker = null;
    TranslucentSystem translucentSystem = null;
    public MenuNode():base("Menu", 0)
    {
    }
    public bool menu_called = false;
    //このNodeをクリックした時点で呼ばれるもの
    public override void  OnClick()
    {
        if(maker == null)
        {
            maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
        }
        if(translucentSystem== null)
        {
            translucentSystem = GameObject.FindObjectOfType<TranslucentSystem>();
        }
        if (translucentSystem.isTranslucent)
        {
            menu_called = false;
        }
        else
        {
            menu_called = !menu_called;
        }
        maker.BreakAllBubbles();
        translucentSystem.isTranslucent = false;
        if (menu_called)
        {
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.System;

namespace Core.UI
{

//SpeechBubbleがそれぞれNodeと紐付けられる
//OnClickでCallbackを呼び、Childrenを読み込み、SpeechBubbleを展開する
//NodeからNodeにメッセージを渡せるといい気がするけどしばらく使わなそうだからいいや
    public class Node 
    {

        public Node parent = null;
        public List<Node> children = new List<Node>();
        protected List<Action> callback = new List<Action>();
        public SpeechBubble speechBubble;
        string _Text = "";
        int mode = 0;
        public Node(string _text, int _mode=0)
        {
            _Text = _text;
            mode = _mode;
        }

        /*
    public void SetBubble(SpeechBubble temp)
    {
        speechBubble = temp;
    }
    */

    
        public void SetParent(Node _parent)
        {
            parent = _parent;
        }
        public void AddChild(Node _child)
        {
            _child.SetParent(this);
            children.Add(_child);
        }
        public void AddChildren(List<Node> _children)
        {
            foreach( Node child in children)
            {
                AddChild(child);
            }
        }
        public void AddCallback(Action _callback)
        {
            callback.Add(_callback);
        }
        //このNodeをクリックした時点で呼ばれるもの
        public virtual void  OnClick()
        {
            foreach(Action _func in callback)
            {
                _func();
            }
            if (children.Count > 0)
            {
                parent?.BreakChildren();
                GenerateChildrenBubble();
            }
        }
        public virtual void OnBreak()
        {
        }
        public virtual void BreakBubble()
        {
            if( speechBubble != null)
            {
                speechBubble.Break();
            }
        }
        public virtual void GenerateBubble(int num =0)
        {
            var maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
            speechBubble = maker.Generate(num, _Text, -1, mode);
            speechBubble.SetNode(this);
        }

        /// <summary>
        /// Is this window transparent
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; speechBubble?.ChangeText(value); }
        }
        public void GenerateChildrenBubble()
        {
            int cnt = 0;
            foreach( Node child in children)
            {
                child.GenerateBubble(cnt);
                cnt++;
            }
        }

        public void BreakChildren()
        {
            foreach( Node child in children)
            {
                child.BreakBubble();
            }
        }
    }
}

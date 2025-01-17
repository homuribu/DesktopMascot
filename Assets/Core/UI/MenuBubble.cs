﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Core.UI
{
    public class MenuBubble : SpeechBubble
    {
        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe(_ =>{
                this.ChangeText(DateTime.Now.ToShortTimeString());
            }).AddTo(this);
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
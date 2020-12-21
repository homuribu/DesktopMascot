using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;
using Kirurobo;

public class SaveSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public SaveData saveData;
    [SerializeField]
    private GameObject obj;
    DataBank bank = null;

    private UniWindowController uniwinc = null;

    void Start()
    {
        uniwinc = GameObject.FindObjectOfType<UniWindowController>();
        
    }
    void Update()
    {
        if (uniwinc == null)
        {
            uniwinc = GameObject.FindObjectOfType<UniWindowController>();
        }
        Load();
        SetWindow();
        enabled = false;
    }

    void Save()
    {
        saveData.x = uniwinc.windowPosition[0];
        saveData.y = uniwinc.windowPosition[1];
        saveData.width = uniwinc.windowSize[0];
        saveData.height = uniwinc.windowSize[1];
        saveData.isTransparent= uniwinc.isTransparent;
        saveData.isTopmost = uniwinc.isTopmost;
        bank.Store("saveData", saveData);
        //Debug.Log(saveData);
        bank.Save("saveData");
    }
    void Load()
    {
        bank = DataBank.Open();

        Debug.Log($"save path of bank is { bank.SavePath }");

        if (bank.Load<SaveData>("saveData"))
        {
            saveData = bank.Get<SaveData>("saveData");
            if (saveData.width == 0 || saveData.height == 0)
            {
                Init();
            }
        }
        else
        {
            Init();
        }

        bank.Store("saveData", saveData);
        //Debug.Log(saveData);

        //Debug.Log("x: "+ saveData.x+ "y: "+ saveData.y+ "w: "+ saveData.width+ "h: "+ saveData.height);
    }
    void Init()
    {
        saveData = new SaveData();
        saveData.x = 500;
        saveData.y = 500;
        saveData.width = 1000;
        saveData.height= 500;
        saveData.isTransparent = true;
        saveData.isTopmost = true;
    }

    /*
    private void OnApplicationFocus(bool focus)
    {

        if (focus)
        {
            if (uniwinc == null)
            {
                uniwinc = GameObject.FindObjectOfType<UniWindowController>();
            }
            Load();
            SetWindow();
        }
    }
    */

    void SetWindow()
    {
        uniwinc.windowSize = new Vector2  (saveData.width, saveData.height );
        uniwinc.windowPosition = new Vector2 ( saveData.x, saveData.y);
        uniwinc.isTransparent = saveData.isTransparent;
        uniwinc.isTopmost= saveData.isTopmost;
        //Debug.Log("SetWindow x: "+ saveData.x+ "y: "+ saveData.y+ "w: "+ saveData.width+ "h: "+ saveData.height);
    }

    void OnApplicationQuit()
    {
        Save();
        //Debug.Log("OnQuit x: "+ saveData.x+ "y: "+ saveData.y+ "w: "+ saveData.width+ "h: "+ saveData.height);
    }
}

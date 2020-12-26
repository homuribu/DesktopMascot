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
    public SaveData originSaveData;
    [SerializeField]
    private GameObject obj;
    DataBank bank = null;

    private UniWindowController uniwinc = null;

    void Start()
    {
    }
    void Update()
    {
        if (uniwinc == null)
        {
            uniwinc = GameObject.FindObjectOfType<UniWindowController>();
            UpdateData(originSaveData);
            Load();
            SetWindow(saveData);
            enabled = false;
        }
    }

    void UpdateData(SaveData data)
    {
        data.x = uniwinc.windowPosition[0];
        data.y = uniwinc.windowPosition[1];
        data.width = uniwinc.windowSize[0];
        data.height = uniwinc.windowSize[1];
        data.isTransparent = uniwinc.isTransparent;
        data.isTopmost = uniwinc.isTopmost;
    }
    void Store(SaveData data)
    {
        bank.Store("saveData", data);
        //Debug.Log(data);
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
                Store(saveData);
            }
        }
        else
        {
            Init();
            Store(saveData);
        }

    }

    void Init()
    {
        saveData = new SaveData();
        saveData.x = 500;
        saveData.y = 500;
        saveData.width = 1000;
        saveData.height = 500;
        saveData.isTransparent = true;
        saveData.isTopmost = true;
    }

    void SetWindow(SaveData data)
    {
        uniwinc.windowSize = new Vector2(data.width, data.height);
        uniwinc.windowPosition = new Vector2(data.x, data.y);
        uniwinc.isTransparent = data.isTransparent;
        uniwinc.isTopmost = data.isTopmost;
        Debug.Log("SetWindow x: " + data.x + "y: " + data.y + "w: " + data.width + "h: " + data.height);
    }

    void OnApplicationQuit()
    {
        UpdateData(saveData);
        Store(saveData);
        Debug.Log("OnQuit x: " + saveData.x + "y: " + saveData.y + "w: " + saveData.width + "h: " + saveData.height);
        originSaveData.isTopmost = false;
        SetWindow(originSaveData);

    }
}

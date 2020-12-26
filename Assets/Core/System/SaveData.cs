using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData : object 
{
    // Start is called before the first frame update
    public float width;
    public float height;

    public float x;
    public float y;

    public bool isTransparent = true;
    public bool isTopmost= true;

    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

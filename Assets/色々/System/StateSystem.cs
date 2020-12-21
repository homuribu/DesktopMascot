using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSystem : MonoBehaviour
{
    GazeController gazeController = null;
    // Start is called before the first frame update
    void Start()
    {
        gazeController = GameObject.FindObjectOfType<GazeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void hoge()
    {
        gazeController.is_enable = true;


    }
}

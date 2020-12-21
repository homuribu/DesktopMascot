using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera _camera;
    
    void Start()
    {
         _camera.backgroundColor = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

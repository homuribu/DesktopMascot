using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBubble : MonoBehaviour
{

    private SpeechBubbleMaker maker;
    // Start is called before the first frame update
    void Start()
    {
        maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TranslucentSystem : MonoBehaviour, IPointerClickHandler
{
    private bool is_translucent = false;
    [SerializeField]
    public float translucent_alpha = 0.7f;

    private CanvasGroup canvasGroup;
    private SpeechBubbleMaker maker = null;
    private MenuNode menuNode = null;
    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        maker = GameObject.FindObjectOfType<SpeechBubbleMaker>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            isTranslucent = !isTranslucent;
        }
    }
    public void SetTranslucent(bool _is_translucent)
    {
        is_translucent = _is_translucent;
        maker.PauseSpeak(is_translucent);
        canvasGroup.alpha =  is_translucent? translucent_alpha: 1.0f;
    }

    public bool isTranslucent 
    {
        get { return is_translucent; }
        set { SetTranslucent(value); }
    }
}

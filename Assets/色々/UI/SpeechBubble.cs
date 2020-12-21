using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UniRx;

public class SpeechBubble : MonoBehaviour, IPointerClickHandler
{


    public Node node=null;
    public Text TargetText;
    public int Count = 0;
    public GameObject Text_obj;
    public RectTransform rect;
    public int strIndex = 0;
    private bool inited = false;

    //Prefab生成するときはStartの前にSpeak等が入る場合があるため、かならず外部からInit()を呼び出すようにする
    public void Init()
    {
        if (!inited)
        {
            Text_obj = this.transform.Find("Text").gameObject;
            TargetText = Text_obj.GetComponent<Text>();
            rect = GetComponent<RectTransform>();
        }
    }

    protected void Start()
    {
        frameCnt = UnityEngine.Random.Range(0, 10000); 
        Init();
    }

        
    private int frameCnt = 0;
    void FixedUpdate()
    {
	    float _amplitude = 0.2f; // 振幅
        frameCnt += 1;
        if (10000 <= frameCnt)
        {
            frameCnt = 0;
        }
        if (0 == frameCnt % 2)
        {
            // 上下に振動させる（ふわふわを表現）
            float posYSin = Mathf.Sin(2.0f * Mathf.PI * (float)(frameCnt % 200) / (200.0f - 1.0f));
            //Debug.Log(posYSin.ToString() +" x " + _amplitude.ToString()+ " = " +(_amplitude * posYSin).ToString());
            iTween.MoveAdd(this.gameObject, new Vector3(0, _amplitude * posYSin, 0), 0.0f);
        }
    }
    public void Speak(string _target, float time)
    {
        ChangeText(_target);
        ShowAnimation();
        if (time >= 0.0f)
        {
            Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
            {
                BackBubble();
                Destroy(this.gameObject,1f);
            }).AddTo(this);
        }

    }
    //吹き出しの種類を変える
    public void SetMode(int mode, bool reverseX=false, bool reverseY=false)
    {
        rect.pivot = new Vector2(reverseX?0:1, reverseY?1:0);
        rect.localPosition -= new Vector3(reverseX?rect.sizeDelta.x:0, reverseY?-rect.sizeDelta.y:0,0);
        SetImage(mode.ToString(), reverseX, reverseY);
    }

    void SetImage(string imageName, bool reverseX=false, bool reverseY=false)
    {

        var Image_obj = this.transform.Find("Image").gameObject;
        var sprite = Resources.Load<Sprite>("Images/SpeechBubble/"+ imageName);
        var image = Image_obj.GetComponent<Image>();
        image.sprite = sprite;
        var _temp = image.transform.localScale;
        _temp.x = reverseX? -1:1;
        _temp.y = reverseY? -1:1;
        image.transform.localScale = _temp;
    }
    void ShowBubble()
    {
        SetAlpha(1.0f);
    }
    void HideBubble()
    {
        SetAlpha(0.0f);
    }

    void SetAlpha(float alpha)
    {
        // iTweenで呼ばれたら、受け取った値をImageのアルファ値にセット
        Vector4 _color = gameObject.GetComponent<UnityEngine.UI.Image>().color;
        _color[3] = alpha;
        gameObject.GetComponent<UnityEngine.UI.Image>().color = _color;

        _color = TargetText.color;
        _color[3] = alpha;
        TargetText.color = _color;
    }
    void SetScaleAdd(float scale, float time, float delay)
    {
        iTween.ScaleAdd(this.gameObject, iTween.Hash(
    "x", scale,
    "y", scale,
    "z", scale,
    "delay", delay,
    "time", time
));
    }
    void SetMoveBy(Vector3 _diff, float _scale, float time, float delay)
    {
        iTween.MoveBy(this.gameObject, iTween.Hash(
    "x", _diff.x * _scale,
    "y", _diff.y * _scale,
    "z", _diff.z * _scale,
    "delay", delay,
    "time", time
));

    }

    void ShowAnimation()
    {
        ShowBubble();
        gameObject.transform.localScale = new Vector3(0,0,0);
        SetScaleAdd(1.0f, 0.4f,0);
    }
    void BackBubble()
    {
        SetScaleAdd(-1.0f, 0.4f,0);
    }

    //はいクソ実装
    public void Break()
    {
        //BackBubble();
        float scale = -1.0f;
        float time = 0.4f;

        iTween.ScaleAdd(this.gameObject, iTween.Hash(
    "x", scale,
    "y", scale,
    "z", scale,
    "time", time,
    "oncomplete", "BreakCompletehandler",
    "oncompletetarget", gameObject
));

    }
    public void BreakCompletehandler()
    {
        Destroy(gameObject);
    }


    public void ChangeText(string _text)
    {
        TargetText.text = _text;
    }
    //相対座標
    Vector3 GetRightBottom()
    {
        var _width = rect.sizeDelta.x;
        var _height = rect.sizeDelta.y;
        var _diff_pos = new Vector3(_width / 2, -_height / 2, 0);
        return _diff_pos;
    }

    void RandomSpeak()
    {

        var strList = new List<string>(){ "おはよう！", "元気?", "やっほー", "おなかすいたー", "がんばれー", "ご飯食べた?"};
        var _index = UnityEngine.Random.Range(0, strList.Count);
        while(strIndex == _index)
        {
            _index = UnityEngine.Random.Range(0, strList.Count);
        }
        strIndex = _index;

        Speak(strList[strIndex], 3);

    }
    public void SetNode(Node _node)
    {
        node = _node;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            var ts = GameObject.FindObjectOfType<TranslucentSystem>();
            ts.isTranslucent = !ts.isTranslucent;
        }
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            node.OnClick();
        }
    }

}

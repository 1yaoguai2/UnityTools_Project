using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class DoTweenExample : MonoBehaviour
{
    public Transform child1;

    [Header("UI")] public TextMeshProUGUI numberTxt;
    public TextMeshProUGUI stringTxt1;
    public Text stringTxt2;
    public Text stringTxt3;

    private string _stringDefault1;
    private string _stringDefault2;

    private void Awake()
    {
        //建议，但也可以不手动初始化
        DOTween.Init();
    }

    void Start()
    {
        _stringDefault1 = stringTxt1.text;
        _stringDefault2 = stringTxt2.text;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(80, 10, 200, 1200), "测试");
        if (GUI.Button(new Rect(100, 50, 150, 30), "重置位置"))
        {
            Reset();
        }

        if (GUI.Button(new Rect(100, 100, 150, 30), "DOMove"))
        {
            //移动
            child1.transform.DOMove(new Vector3(2, 2, 2), 2f, false); 
        }

        if (GUI.Button(new Rect(100, 150, 150, 30), "DOMove(snapping = true)"))
        {
            //整数运行，瞬移
            child1.transform.DOMove(new Vector3(-2, 2, -2), 2f, true); 
        }

        if (GUI.Button(new Rect(100, 200, 150, 30), "DOMoveY"))
        {
            //单轴移动
            child1.transform.DOMoveY(-3, 2f);
        }

        if (GUI.Button(new Rect(100, 250, 150, 30), "DOLocalMove"))
        {
            //按本地坐标移动
            child1.transform.DOLocalMove(new Vector3(3, 3, 3), 2f);
        }

        if (GUI.Button(new Rect(100, 300, 150, 30), "DORotate"))
        {
            //旋转
            child1.transform.DORotate(new Vector3(0, 180f, 0), 2f);
        }
        
        if (GUI.Button(new Rect(100, 350, 150, 30), "DORotateQuaternion"))
        {
            //旋转，欧拉角
            var target = Quaternion.Euler(new Vector3(0, 270f, 0));
            child1.transform.DORotateQuaternion(target, 2f);
        }
        
        if (GUI.Button(new Rect(100, 400, 150, 30), "缩放"))
        {
            //缩放
            child1.transform.DOScale(new Vector3(2, 2, 2), 2f);
        }

        if (GUI.Button(new Rect(100, 450, 150, 30), "使用To数字变化"))
        {
            DOTween.To((value) =>
            {
                numberTxt.text = value.ToString("N0");
            }, 0f, 100000f, 3f).SetEase(Ease.Flash);
        }

        if (GUI.Button(new Rect(100, 500, 150, 30), "使用To颜色变化"))
        {
            DOTween.To((value) =>
            {
                numberTxt.color = Color.Lerp(Color.blue, Color.red, value);

            }, 0, 1, 3f).SetEase(Ease.Flash);

            //numberTxt.DOColor(Color.red, 3f);
        }

        if (GUI.Button(new Rect(100,550,150,30),"逐字显示"))
        {
            //清空后逐字写入
            DOTween.To(() => string.Empty, value => stringTxt1.text = value,"DoTween插件真好用！",3f).SetEase(Ease.Linear)
                .SetOptions(true); //SetOptions-富文本
            //逐字打印，覆盖原本有的字符
            stringTxt2.DOText("DoTween插件真好用！", 3f);
            //逐字打印
            stringTxt3.DOText("DoTween插件真好用！", 3f);
        }
    }

    //重置Transform数据
    private void Reset()
    {
        child1.transform.ResetTransform();
        numberTxt.text = "99999";
        numberTxt.color = Color.white;
        stringTxt1.text = _stringDefault1;
        stringTxt2.text = _stringDefault2;
        stringTxt3.text = string.Empty;
    }

    /*
    DOMove
    /// <summary>
    ///
    /// </summary>
    /// <param name="to">目标位置（世界或者本地）</param>
    /// <param name="duration">持续时间</param>
    /// <param name="snapping">默认false，设为true时，自动平滑地将所有值变为整数，每次移动整数</param>
    //transform.DoMove(Vector3 to, float duration, bool snapping){}
    //transform.DoLocalMove(Vector3 to, float duration, bool snapping){}

    //DoMoveX/DoMoveY/DoMoveZ
    //DoJump
    */
}
using System;
using UnityEngine;
using XTools.UI;

/// <summary>
/// 基于GUI的弹窗界面
/// </summary>
public class ConfirmWindowGUI : BasePanel
{
    private bool _isShowGUI;
    private string _tipStr;
    private Action _confirmEvent;
    private Action _cancelEvent;

    private void OnGUI()
    {
        if (_isShowGUI)
        {
            float w = SceneWindowManager.Instance.resolutionRatioWidth / 2f;
            float h = SceneWindowManager.Instance.resolutionRatioHeight / 2f;

            GUI.Box(new Rect(w - 200, h - 100, 400, 200), _tipStr);

            if (GUI.Button(new Rect(w - 150,h,100,50),"确认"))
            {
                _confirmEvent?.Invoke();
            }

            if (GUI.Button(new Rect(w + 50, h, 100, 50), "取消"))
            {
                _cancelEvent?.Invoke();
                ClosePanel();
            }
        }
    }

    public void LoadConfirmWindowGUI(string tip, Action confirmEvent, Action cancelEvent)
    {
        _tipStr = tip;
        if (confirmEvent is null)
        {
            _confirmEvent = ClosePanel;
        }
        else
        {
            _confirmEvent = confirmEvent;
            if (cancelEvent is not null)
            {
                _cancelEvent = cancelEvent;
            }
        }

        _isShowGUI = true;
    }
}
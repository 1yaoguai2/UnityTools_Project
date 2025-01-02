using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XTools.UI;

/// <summary>
/// 基于UGUI的弹窗界面
/// </summary>
public class ConfirmWindow : BaseScalePanel
{
    public TextMeshProUGUI tipTxt;
    public Button btnConfirm;
    public Button btnCancel;

    private RectTransform confirmButtonRect;
    private RectTransform cancelButtonRect;
    private Vector3 confirmButtonDefaultlocalPos;
    private Vector3 cancelButtonDefaultlocalPos;
    private void Start()
    {
        closeEvent = OnClosePanel;
        confirmButtonRect = btnConfirm.GetComponent<RectTransform>();
        confirmButtonDefaultlocalPos = confirmButtonRect.localPosition;
        cancelButtonRect = btnCancel.GetComponent<RectTransform>();
        cancelButtonDefaultlocalPos = cancelButtonRect.localPosition;
    }


    public void LoadConfirmWindow(string tip, UnityAction confirmEvent, UnityAction cancelEvent)
    {
        tipTxt.text = tip;
        if (confirmEvent is null)
        {
            cancelButtonRect.gameObject.SetActive(false);
            confirmButtonRect.localPosition = new Vector3(0,confirmButtonDefaultlocalPos.y,confirmButtonDefaultlocalPos.z);
            btnConfirm.onClick.AddListener(ClosePanel);
        }
        else
        {
           btnConfirm.onClick.AddListener(confirmEvent);
           if (cancelEvent is not null)
           {
               btnCancel.onClick.AddListener(cancelEvent);
           }
           btnCancel.onClick.AddListener(ClosePanel);
        }

    }

    private void OnClosePanel()
    {
        confirmButtonRect.localPosition = confirmButtonDefaultlocalPos;
        cancelButtonRect.localPosition = cancelButtonDefaultlocalPos;
        cancelButtonRect.gameObject.SetActive(true);
        btnConfirm.onClick.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
    }
}
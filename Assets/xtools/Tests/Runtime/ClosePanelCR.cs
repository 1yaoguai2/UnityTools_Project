using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using InputSysteam;
using UnityEngine.InputSystem;
#endif

public class ClosePanelCR : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    private AllCanvasController _allCanvasController;
    private InputControls _inputController;

    private void Awake()
    {
        _allCanvasController = GetComponent<AllCanvasController>();
        _inputController = new InputControls();
        _inputController.Player.UI.started += ClosePanel;
    }


    private void OnEnable()
    {
        _inputController.Enable();
    }

    private void OnDisable()
    {
        _inputController.Disable();
    }


    private void ClosePanel(InputAction.CallbackContext obj)
    {
        CustomLogger.Log($"键盘按键按下{obj.control.name}");
        if (obj.control.name.Equals("escape"))
        {
            _allCanvasController.CloseSomePanel();
        }
    }
#endif
}
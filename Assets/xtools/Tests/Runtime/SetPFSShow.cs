using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using InputSysteam;
using UnityEngine.InputSystem;
#endif

public class SetPFSShow : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    private InputControls _inputController;

    private GetFPS _getFPS;

    private void Awake()
    {
        _getFPS = GetComponent<GetFPS>();
        _inputController = new InputControls();
        _inputController.Player.UI.started += ShowFPSCR;
    }


    private void OnEnable()
    {
        _inputController.Enable();
    }

    private void OnDisable()
    {
        _inputController.Disable();
    }


    private void ShowFPSCR(InputAction.CallbackContext obj)
    {
        CustomLogger.Log($"键盘按键按下{obj.control.name}");
        if (obj.control.name.Equals("f8"))
        {
            _getFPS.ShowFPSControl();
        }
    }
#endif
}
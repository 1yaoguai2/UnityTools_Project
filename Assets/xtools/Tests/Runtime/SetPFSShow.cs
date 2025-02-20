using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SetPFSShow : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    private PlayerInputController _inputController;

    private GetFPS _getFPS;

    private void Awake()
    {
        _getFPS = GetComponent<GetFPS>();
        _inputController = new PlayerInputController();
        _inputController.Player.UICR.started += ShowFPSCR;
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
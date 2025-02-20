using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
#endif
public class MainCameraNewInputController : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    [Header("Movement Settings")]
    [SerializeField]
    float moveSpeed = 10f;

    [SerializeField]
    float ascendSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField]
    float rotationSpeed = 0.2f;

    [SerializeField]
    float minPitch = -90f;

    [SerializeField]
    float maxPitch = 90f;

    [Header("Pan Settings")]
    [SerializeField]
    float panSpeed = 2f;

    [Header("Zoom Settings")]
    [SerializeField]
    float zoomSensitivity = 2f;

    private Vector2 moveInput;
    private float verticalInput;
    private float zoomInput;
    private float rotationY;
    private float rotationX;

    private InputAction moveAction;
    private InputAction ascendDescendAction;
    private InputAction zoomAction;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        InputActionMap map = playerInput.actions.FindActionMap("Camera");

        moveAction = map.FindAction("Move");
        ascendDescendAction = map.FindAction("AscendDescend");
        zoomAction = map.FindAction("Zoom");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        ascendDescendAction.Enable();
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        ascendDescendAction.Disable();
        zoomAction.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleVertical();
        HandleRotation();
        HandlePan();
        HandleZoom();
    }

    //移动
    void HandleMovement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        movement = transform.TransformDirection(movement) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    //升降
    void HandleVertical()
    {
        verticalInput = ascendDescendAction.ReadValue<float>();
        transform.position += Vector3.up * verticalInput * ascendSpeed * Time.deltaTime;
    }

    //视角旋转
    void HandleRotation()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            rotationY += delta.x * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX - delta.y * rotationSpeed, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }

    //鼠标中键拖动
    void HandlePan()
    {
        if (Mouse.current.middleButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            Vector3 pan = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;
            transform.Translate(pan, Space.Self);
        }
    }

    //滚轮滚动
    void HandleZoom()
    {
        zoomInput = zoomAction.ReadValue<float>();
        transform.position += transform.forward * zoomInput * zoomSensitivity * Time.deltaTime;
    }
#endif
}
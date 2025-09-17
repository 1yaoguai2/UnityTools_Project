using System;
using UnityEngine;

namespace CameraRender
{
    public class CameraRender : MonoBehaviour
    {
        [Header("Camera Settings")]
        public Transform center;
        public float rotationSpeed = 360f;
        public float inertiaDamping = 0.75f;
        public Vector2 distanceRange = new Vector2(0.5f, 10f);
        [Header("Rotation Constraints")]
        public Vector2 verticalAngleRange = new Vector2(1f, 89f);

        public static CameraRender Instance { get; private set; }

        private bool _isEnabled = true;

        // Camera parameters
        private Vector3 _origin;
        private float _radius;
        private float _eulerX, _eulerY;

        // Touch/input handling
        private Vector2 _screenCoord;
        private Vector2 _coordDelta;
        private Vector2 _prevCoordDelta;
        private Vector2 _coordDDelta;
        private float _touchBeginEulerX, _touchBeginEulerY;
        private bool _isDragging;
        private int _lastDragFrame;

        // Screen dimensions for rotation calculation
        private float _width, _height;
        private float _eulerDeltaMulX, _eulerDeltaMulY;

        private void Start()
        {
            InitializeCamera();
            Instance = this;
            
            // 默认设置屏幕尺寸为屏幕分辨率
            SetScreenSize(Screen.width, Screen.height);
        }

        private void Update()
        {
            if (!_isEnabled) return;

            // 处理鼠标滚轮缩放
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(scrollDelta) > 0.01f)
            {
                OnZoom(scrollDelta);
            }
        }

        private void LateUpdate()
        {
            if (!_isEnabled) return;

            // 处理鼠标拖拽
            HandleMouseInput();

            if (_isDragging)
            {
                // 如果拖拽动作停止了也会及时计算鼠标位置增量
                if (Math.Abs(Time.frameCount - _lastDragFrame) > 1)
                {
                    // 这里可以添加额外的惯性计算逻辑
                }
            }
            else
            {
                // 处理惯性效果
                if (Math.Abs(_coordDDelta.x + _coordDDelta.y) > 0.5f)
                {
                    var eulerDelta = _coordDDelta;
                    eulerDelta.x *= _eulerDeltaMulX;
                    eulerDelta.y *= _eulerDeltaMulY;
                    _eulerX += eulerDelta.y;
                    _eulerY += eulerDelta.x;
                    _eulerX = Mathf.Clamp(_eulerX, verticalAngleRange.x, verticalAngleRange.y); // 限制角度范围
                    ApplyCameraTransform();


                    _coordDDelta.x *= inertiaDamping;
                    _coordDDelta.y *= inertiaDamping;
                }
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnDragStart(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                OnDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnDragEnd();
            }
        }

        private void InitializeCamera()
        {
            UpdateDistance();
            var euler = transform.localEulerAngles;
            _eulerX = euler.x;
            _eulerY = euler.y;
            _eulerX = Mathf.Clamp(_eulerX, verticalAngleRange.x, verticalAngleRange.y); // 添加这一行
            ApplyCameraTransform();
        }

        private void UpdateDistance()
        {
            if (center == null) return;
            
            _origin = center.transform.position;
            _radius = Vector3.Distance(_origin, transform.position);
        }

        private void ApplyCameraTransform()
        {
            if (center == null) return;
            
            var rotation = Quaternion.Euler(_eulerX, _eulerY, 0);
            var position = _radius * (rotation * Vector3.back) + _origin;
            transform.SetPositionAndRotation(position, rotation);
        }

        public void SetScreenSize(float width, float height)
        {
            _width = width;
            _height = height;
            _eulerDeltaMulX = rotationSpeed / _width;
            _eulerDeltaMulY = rotationSpeed / _height / 2f;
        }

        public void EnableCamera()
        {
            _isEnabled = true;
            gameObject.SetActive(true);
        }

        public void DisableCamera()
        {
            _isEnabled = false;
            _isDragging = false;
            gameObject.SetActive(false);
        }

        public void StopInertia()
        {
            _coordDDelta = Vector2.zero;
        }

        // Mouse/touch input handlers
        public void OnDragStart(Vector2 screenPosition)
        {
            _screenCoord = screenPosition;
            _prevCoordDelta = Vector2.zero;
            _touchBeginEulerX = _eulerX;
            _touchBeginEulerY = _eulerY;
            StopInertia();
            ApplyCameraTransform();
            _isDragging = true;
        }

        public void OnDrag(Vector2 currentScreenPosition)
        {
            _coordDelta = currentScreenPosition - _screenCoord;

            // 防止突变
            if (!IsDeltaBurst(_coordDelta - _prevCoordDelta))
            {
                _coordDDelta = _coordDelta - _prevCoordDelta;
                _prevCoordDelta = _coordDelta;
            }

            _lastDragFrame = Time.frameCount;

            // 像素偏差 => 角度偏差
            var eulerDelta = _coordDelta;
            eulerDelta.x *= _eulerDeltaMulX;
            eulerDelta.y *= _eulerDeltaMulY;

            _eulerX = _touchBeginEulerX + eulerDelta.y;
            _eulerY = _touchBeginEulerY + eulerDelta.x;
            _eulerX = Mathf.Clamp(_eulerX, verticalAngleRange.x, verticalAngleRange.y); // 限制角度范围
            ApplyCameraTransform();
        }

        public void OnDragEnd()
        {
            _isDragging = false;
        }

        public void OnZoom(float delta)
        {
            if (delta > 0)
            {
                // 靠近
                _radius *= 0.8f;
            }
            else if (delta < 0)
            {
                // 远离
                _radius *= 1.2f;
            }

            _radius = Mathf.Clamp(_radius, distanceRange.x, distanceRange.y);
            ApplyCameraTransform();
        }

        private bool IsDeltaBurst(Vector2 delta)
        {
            const float maxDDelta = 400f;
            return Mathf.Abs(delta.x) > maxDDelta || Mathf.Abs(delta.y) > maxDDelta;
        }
    }
}

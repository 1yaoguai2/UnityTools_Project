#region

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    public class TopEdgeSnapWindow : MonoBehaviour
    {
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private const int SnapThreshold = 10;
        private const int HiddenOffset = 2;
        private const float AnimationSpeed = 10f;
        private const int TriggerDistance = 20;

        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new(-2);

        private IntPtr _curState;
        private bool _isAnimating;
        private bool _isHidden;
        private bool _isSnapped;
        private Vector2 _targetPosition;

        private IntPtr _windowHandle;
        private RECT _windowRect;

        private void Start()
        {
            _windowHandle = FindWindow(null, Application.productName);
            if (_windowHandle == IntPtr.Zero)
            {
                Debug.LogError("Could not find game window handle.");
                return;
            }

            Debug.Log("Window handle: " + _windowHandle);
        }

        private void Update()
        {
            if (_windowHandle == IntPtr.Zero)
                return;

            GetWindowRect(_windowHandle, out _windowRect);

            if (!_isAnimating) CheckSnapToTop();

            CheckMouseEnterOrLeave();

            if (_isAnimating) AnimateWindow();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        private void CheckSnapToTop()
        {
            var wasSnapped = _isSnapped;
            _isSnapped = Math.Abs(_windowRect.Top) < SnapThreshold;

            if (_isSnapped && !wasSnapped)
            {
                SnapToTop();
                SetAlwaysOnTop(true);
            }
            else if (!_isSnapped && wasSnapped)
            {
                SetAlwaysOnTop(false);
            }
        }

        private void SetAlwaysOnTop(bool isTopmost)
        {
            if (isTopmost && _curState != HWND_TOPMOST)
            {
                _curState = HWND_TOPMOST;
                SetWindowPos(_windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            }
            else if (!isTopmost && _curState != HWND_NOTOPMOST)
            {
                _curState = HWND_NOTOPMOST;
                SetWindowPos(_windowHandle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            }
        }

        private void SnapToTop()
        {
            SetWindowPos(_windowHandle, IntPtr.Zero, _windowRect.Left, 0, _windowRect.Right - _windowRect.Left, _windowRect.Bottom - _windowRect.Top, 0);
            _isSnapped = true;
        }

        private void CheckMouseEnterOrLeave()
        {
            if (!_isSnapped)
                return;

            POINT cursorPos;
            GetCursorPos(out cursorPos);

            var isMouseNearTop = cursorPos.Y <= HiddenOffset + TriggerDistance;

            var isMouseInsideWindow = cursorPos.X >= _windowRect.Left && cursorPos.X <= _windowRect.Right &&
                                      cursorPos.Y >= _windowRect.Top && cursorPos.Y <= _windowRect.Bottom;

            if (isMouseNearTop && _isHidden && isMouseInsideWindow)
                RestoreWindowWithAnimation();
            else if (!isMouseInsideWindow && !_isHidden) HideWindowWithAnimation();
        }

        private void HideWindowWithAnimation()
        {
            if (!_isSnapped)
                return;

            _targetPosition = new Vector2(_windowRect.Left, -(_windowRect.Bottom - _windowRect.Top - HiddenOffset));
            _isAnimating = true;
            _isHidden = true;
        }

        private void RestoreWindowWithAnimation()
        {
            _targetPosition = new Vector2(_windowRect.Left, 0);
            _isAnimating = true;
            _isHidden = false;
        }

        private void AnimateWindow()
        {
            var currentPosition = new Vector2(_windowRect.Left, _windowRect.Top);
            var newPosition = Vector2.Lerp(currentPosition, _targetPosition, Time.deltaTime * AnimationSpeed);

            SetWindowPos(_windowHandle, IntPtr.Zero, (int)newPosition.x, (int)newPosition.y, _windowRect.Right - _windowRect.Left, _windowRect.Bottom - _windowRect.Top,
                0);

            if (Vector2.Distance(currentPosition, _targetPosition) < 1f) _isAnimating = false;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
    }
}
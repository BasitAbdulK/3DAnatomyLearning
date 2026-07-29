using System.Collections.Generic;
using SimFusion.Anatomy.Core;
using SimFusion.Anatomy.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimFusion.Anatomy.CameraControl
{
    /// Reads touch input and exposes it as normalised camera deltas.
    /// One finger = orbit, two-finger pinch = zoom, two-finger drag = pan.
    public class TouchCameraInput : MonoBehaviour, ICameraInputProvider
    {
        [SerializeField] private CameraSettings settings;

        private Vector2 _orbitDelta;
        private Vector2 _panDelta;
        private float   _zoomDelta;

        private float _previousPinchDistance;
        private bool  _pinchActive;

        // Finger IDs whose touch began over a UI element. Cleared on Ended/Canceled.
        private readonly HashSet<int> _uiTouches = new HashSet<int>();

        // Drag threshold state for single-finger orbit.
        // _orbitUnlocked stays false until the finger has travelled at least
        // settings.TouchDragThreshold pixels from _touchStartPosition.
        // This prevents micro-movement during a tap from producing an orbit delta.
        private Vector2 _touchStartPosition;
        private bool    _orbitUnlocked;

        // ── Lifecycle ─────────────────────────────────────────────────────────

        private void Update()
        {
            _orbitDelta = Vector2.zero;
            _panDelta   = Vector2.zero;
            _zoomDelta  = 0f;

            UpdateUITouchRegistry();

            int touchCount = Input.touchCount;

            if (touchCount == 1)
                HandleSingleTouch();
            else if (touchCount == 2)
                HandleTwoTouches();
            else
                _pinchActive = false;
        }

        // ── UI touch registry ─────────────────────────────────────────────────

        // Iterates every active touch each frame:
        //   Began  → register if over UI.
        //   Ended/Canceled → release, so camera control resumes immediately.
        private void UpdateUITouchRegistry()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        if (IsPointerOverUI(t.fingerId))
                            RegisterUITouch(t.fingerId);
                        // Record start position so HandleSingleTouch can measure
                        // travel distance before unlocking orbit.
                        _touchStartPosition = t.position;
                        _orbitUnlocked      = false;
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        ReleaseUITouch(t.fingerId);
                        break;
                }
            }
        }

        private void RegisterUITouch(int fingerId)  => _uiTouches.Add(fingerId);
        private void ReleaseUITouch(int fingerId)   => _uiTouches.Remove(fingerId);
        private bool IsUIOwnedTouch(int fingerId)   => _uiTouches.Contains(fingerId);

        private static bool IsPointerOverUI(int fingerId)
            => EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject(fingerId);

        // ── Gesture handlers ──────────────────────────────────────────────────

        private void HandleSingleTouch()
        {
            _pinchActive = false;
            Touch touch = Input.GetTouch(0);

            if (IsUIOwnedTouch(touch.fingerId))
                return;

            if (touch.phase != TouchPhase.Moved)
                return;

            // Once unlocked, stay unlocked for the lifetime of this touch so the
            // orbit does not stutter if the finger briefly slows below the threshold.
            if (!_orbitUnlocked)
            {
                float travel = Vector2.Distance(touch.position, _touchStartPosition);
                if (travel < settings.DragThresholdPixels)
                    return;

                _orbitUnlocked = true;
            }

            _orbitDelta = touch.deltaPosition * settings.TouchOrbitSensitivity;
        }

        private void HandleTwoTouches()
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // If either finger owns a UI touch, abort the gesture and reset pinch
            // state to prevent a position jump when the finger is eventually released.
            if (IsUIOwnedTouch(t0.fingerId) || IsUIOwnedTouch(t1.fingerId))
            {
                _pinchActive = false;
                return;
            }

            float currentPinchDistance = Vector2.Distance(t0.position, t1.position);

            if (!_pinchActive)
            {
                _previousPinchDistance = currentPinchDistance;
                _pinchActive = true;
                return;
            }

            _zoomDelta = (currentPinchDistance - _previousPinchDistance)
                       * settings.TouchPinchSensitivity;
            _previousPinchDistance = currentPinchDistance;

            Vector2 prevMidpoint = ((t0.position - t0.deltaPosition) + (t1.position - t1.deltaPosition)) * 0.5f;
            Vector2 currMidpoint = (t0.position + t1.position) * 0.5f;
            _panDelta = (currMidpoint - prevMidpoint) * settings.TouchPanSensitivity;
        }

        // ── ICameraInputProvider ──────────────────────────────────────────────

        public Vector2 GetOrbitDelta() => _orbitDelta;
        public float   GetZoomDelta()  => _zoomDelta;
        public Vector2 GetPanDelta()   => _panDelta;
    }
}

using System;
using SimFusion.Anatomy.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimFusion.Anatomy.Interaction
{
    /// Converts raw pointer/touch input into selection events.
    /// Responsible only for input reading, raycasting, and raising events.
    /// Has no knowledge of selection state, highlighting, or UI content.

    public class InputRaycaster : MonoBehaviour
    {
        [Header("Raycast")]
        [Tooltip("Camera used to build the selection ray. Defaults to Camera.main if left empty.")]
        [SerializeField] private Camera raycastCamera;

        [Tooltip("Only GameObjects on these layers can be selected.")]
        [SerializeField] private LayerMask partLayerMask = ~0;

        [Tooltip("Maximum ray distance in world units.")]
        [SerializeField, Min(0.1f)] private float maxSelectionDistance = 100f;

        [Header("Tap Detection")]
        [Tooltip("Shared drag threshold. Must be the same ScriptableObject assigned to TouchCameraInput " +
                 "so both systems use an identical tap-vs-drag boundary.")]
        [SerializeField] private CameraSettings cameraSettings;

        public event Action<PartSelectable> PartClicked;
        public event Action                 EmptySpaceClicked;

        // Pre-allocated buffer — Physics.RaycastNonAlloc avoids heap allocation per cast.
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[1];

        // Touch tap state tracked across frames.
        private Vector2 _touchBeganPosition;
        private bool    _touchInvalidatedByGesture;

        private void Awake()
        {
            if (raycastCamera == null)
                raycastCamera = Camera.main;
        }

        private void Update()
        {
            // Invalidate tap the moment a second finger appears (pinch / pan gesture).
            if (Input.touchCount > 1)
                _touchInvalidatedByGesture = true;

            if (TryGetMouseClick(out Vector2 mousePos))
                ProcessPointer(mousePos, fingerId: -1);
            else if (TryGetTouchTap(out Vector2 tapPos, out int fingerId))
                ProcessPointer(tapPos, fingerId);
        }

        // ── Input helpers ────────────────────────────────────────────────────

        private bool TryGetMouseClick(out Vector2 screenPos)
        {
            if (Input.GetMouseButtonUp(0))
            {
                screenPos = Input.mousePosition;
                return true;
            }
            screenPos = default;
            return false;
        }

        private bool TryGetTouchTap(out Vector2 screenPos, out int fingerId)
        {
            screenPos = default;
            fingerId  = -1;

            if (Input.touchCount != 1)
                return false;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchBeganPosition         = touch.position;
                    _touchInvalidatedByGesture  = false;
                    break;

                case TouchPhase.Ended:
                    if (_touchInvalidatedByGesture)
                        break;
                    float drag = Vector2.Distance(touch.position, _touchBeganPosition);
                    if (drag > cameraSettings.DragThresholdPixels)
                        break;
                    screenPos = touch.position;
                    fingerId  = touch.fingerId;
                    return true;
            }
            return false;
        }

        // ── Raycast ──────────────────────────────────────────────────────────

        private void ProcessPointer(Vector2 screenPos, int fingerId)
        {
            if (IsPointerOverUI(screenPos, fingerId))
                return;

            Ray ray = raycastCamera.ScreenPointToRay(screenPos);
            int hits = Physics.RaycastNonAlloc(ray, _hitBuffer, maxSelectionDistance, partLayerMask);

            if (hits > 0)
            {
                PartSelectable selectable = _hitBuffer[0].collider.GetComponent<PartSelectable>();
                if (selectable != null)
                    PartClicked?.Invoke(selectable);
                else
                    EmptySpaceClicked?.Invoke();
            }
            else
            {
                EmptySpaceClicked?.Invoke();
            }
        }

        private bool IsPointerOverUI(Vector2 screenPos, int fingerId)
        {
            if (EventSystem.current == null)
                return false;

            return fingerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(fingerId)
                : EventSystem.current.IsPointerOverGameObject();
        }
    }
}

using SimFusion.Anatomy.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimFusion.Anatomy.CameraControl
{
    /// <summary>
    /// Reads mouse input and exposes it as normalised camera deltas.
    /// Left-drag = orbit, right-drag = pan, scroll wheel = zoom.
    /// Camera input is suppressed while the pointer is over a UI element.
    /// </summary>
    public class PCCameraInput : MonoBehaviour, ICameraInputProvider
    {
        private Vector2 _lastMousePos;
        private Vector2 _orbitDelta;
        private Vector2 _panDelta;
        private float   _zoomDelta;

        private void Update()
        {
            _orbitDelta = Vector2.zero;
            _panDelta   = Vector2.zero;
            _zoomDelta  = 0f;

            Vector2 currentMousePos = Input.mousePosition;

            // Always update _lastMousePos so there is no position jump when
            // the pointer moves off a UI element back into the 3D viewport.
            bool overUI = EventSystem.current != null
                       && EventSystem.current.IsPointerOverGameObject();

            if (!overUI)
            {
                if (Input.GetMouseButton(0))
                    _orbitDelta = currentMousePos - _lastMousePos;

                if (Input.GetMouseButton(1))
                    _panDelta = currentMousePos - _lastMousePos;

                _zoomDelta = Input.mouseScrollDelta.y;
            }

            _lastMousePos = currentMousePos;
        }

        public Vector2 GetOrbitDelta() => _orbitDelta;
        public float   GetZoomDelta()  => _zoomDelta;
        public Vector2 GetPanDelta()   => _panDelta;
    }
}

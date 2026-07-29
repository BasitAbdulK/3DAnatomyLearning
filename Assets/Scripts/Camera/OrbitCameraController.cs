using SimFusion.Anatomy.Core;
using SimFusion.Anatomy.Data;
using UnityEngine;

namespace SimFusion.Anatomy.CameraControl
{
    /// Orbits the camera around a pivot using yaw/pitch/distance.
    /// Consumes normalised deltas from an ICameraInputProvider — knows nothing about mice or touches.

    public class OrbitCameraController : MonoBehaviour
    {
        [SerializeField] private Transform        pivot;
        [SerializeField] private CameraSettings   settings;
        [SerializeField] private MonoBehaviour    inputProviderSource;

        private ICameraInputProvider _input;

        // Current smoothed state
        private float   _yaw;
        private float   _pitch;
        private float   _distance;
        private Vector3 _pivotOffset;   // accumulated pan offset in world space

        // Target state (what we lerp toward)
        private float   _targetYaw;
        private float   _targetPitch;
        private float   _targetDistance;
        private Vector3 _targetPivotOffset;

        private void Awake()
        {
            _input = inputProviderSource as ICameraInputProvider;

            _yaw      = _targetYaw      = settings.DefaultYaw;
            _pitch    = _targetPitch    = settings.DefaultPitch;
            _distance = _targetDistance = settings.DefaultDistance;
            _pivotOffset = _targetPivotOffset = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (_input == null) return;

            ApplyOrbitInput();
            ApplyZoomInput();
            ApplyPanInput();
            SmoothAndPositionCamera();
        }

        private void ApplyOrbitInput()
        {
            Vector2 delta = _input.GetOrbitDelta();
            if (delta == Vector2.zero) return;

            _targetYaw   += delta.x * settings.OrbitSpeed * Time.deltaTime;
            _targetPitch -= delta.y * settings.OrbitSpeed * Time.deltaTime;
            _targetPitch  = Mathf.Clamp(_targetPitch, settings.MinPitch, settings.MaxPitch);
        }

        private void ApplyZoomInput()
        {
            float delta = _input.GetZoomDelta();
            if (Mathf.Approximately(delta, 0f)) return;

            _targetDistance -= delta * settings.ZoomSpeed;
            _targetDistance  = Mathf.Clamp(_targetDistance, settings.MinDistance, settings.MaxDistance);
        }

        private void ApplyPanInput()
        {
            Vector2 delta = _input.GetPanDelta();
            if (delta == Vector2.zero) return;

            // Pan in the camera's local XY plane
            Vector3 right = transform.right;
            Vector3 up    = transform.up;
            _targetPivotOffset -= (right * delta.x + up * delta.y) * settings.PanSpeed * Time.deltaTime;
        }

        private void SmoothAndPositionCamera()
        {
            float dt = Time.deltaTime;

            _yaw          = Mathf.LerpAngle(_yaw,     _targetYaw,          settings.OrbitSmoothing * dt);
            _pitch        = Mathf.LerpAngle(_pitch,   _targetPitch,        settings.OrbitSmoothing * dt);
            _distance     = Mathf.Lerp(_distance,     _targetDistance,     settings.ZoomSmoothing  * dt);
            _pivotOffset  = Vector3.Lerp(_pivotOffset, _targetPivotOffset, settings.PanSmoothing   * dt);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3    pivotPos = (pivot != null ? pivot.position : Vector3.zero) + _pivotOffset;
            transform.position = pivotPos + rotation * new Vector3(0f, 0f, -_distance);
            transform.rotation = rotation;
        }

        /// <summary>Called by CameraResetService to snap targets back to defaults.</summary>
        public void ResetTargetsToDefaults()
        {
            _targetYaw          = settings.DefaultYaw;
            _targetPitch        = settings.DefaultPitch;
            _targetDistance     = settings.DefaultDistance;
            _targetPivotOffset  = Vector3.zero;
        }
    }
}

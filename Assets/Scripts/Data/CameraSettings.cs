using UnityEngine;

namespace SimFusion.Anatomy.Data
{
    [CreateAssetMenu(menuName = "Anatomy/Camera Settings", fileName = "CameraSettings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("Distance")]
        [SerializeField] private float defaultDistance = 3.5f;
        [SerializeField] private float minDistance     = 0.8f;
        [SerializeField] private float maxDistance     = 8f;

        [Header("Default Angles (degrees)")]
        [SerializeField] private float defaultYaw   = 0f;
        [SerializeField] private float defaultPitch = 20f;

        [Header("Vertical Clamp (degrees)")]
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch =  80f;

        [Header("Orbit")]
        [SerializeField] private float orbitSpeed     = 180f;
        [SerializeField] private float orbitSmoothing = 12f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed     = 4f;
        [SerializeField] private float zoomSmoothing = 10f;

        [Header("Pan")]
        [SerializeField] private float panSpeed     = 1.5f;
        [SerializeField] private float panSmoothing = 12f;

        [Header("Touch Sensitivity")]
        [SerializeField] private float touchOrbitSensitivity = 0.2f;
        [SerializeField] private float touchPinchSensitivity = 0.02f;
        [SerializeField] private float touchPanSensitivity   = 0.004f;

        [Tooltip("Minimum finger travel in pixels that distinguishes a drag from a tap. " +
                 "Used by both TouchCameraInput (orbit unlock) and InputRaycaster (tap detection) " +
                 "so the two systems share a single consistent boundary.")]
        [SerializeField, Min(0f)] private float dragThresholdPixels = 15f;

        [Header("Reset")]
        [SerializeField] private float resetDuration = 0.6f;

        public float DefaultDistance       => defaultDistance;
        public float MinDistance           => minDistance;
        public float MaxDistance           => maxDistance;
        public float DefaultYaw            => defaultYaw;
        public float DefaultPitch          => defaultPitch;
        public float MinPitch              => minPitch;
        public float MaxPitch              => maxPitch;
        public float OrbitSpeed            => orbitSpeed;
        public float OrbitSmoothing        => orbitSmoothing;
        public float ZoomSpeed             => zoomSpeed;
        public float ZoomSmoothing         => zoomSmoothing;
        public float PanSpeed              => panSpeed;
        public float PanSmoothing          => panSmoothing;
        public float TouchOrbitSensitivity => touchOrbitSensitivity;
        public float TouchPinchSensitivity => touchPinchSensitivity;
        public float TouchPanSensitivity   => touchPanSensitivity;
        public float DragThresholdPixels    => dragThresholdPixels;
        public float ResetDuration         => resetDuration;
    }
}

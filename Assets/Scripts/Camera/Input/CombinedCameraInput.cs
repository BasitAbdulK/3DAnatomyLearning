using SimFusion.Anatomy.Core;
using UnityEngine;

namespace SimFusion.Anatomy.CameraControl
{
    /// Aggregates PC and touch input providers into one ICameraInputProvider.
    /// Each provider returns zero when its platform is inactive, so summing is always safe.
    /// Assign this as the inputProviderSource on OrbitCameraController.
    public class CombinedCameraInput : MonoBehaviour, ICameraInputProvider
    {
        [SerializeField] private PCCameraInput    pcInput;
        [SerializeField] private TouchCameraInput touchInput;

        public Vector2 GetOrbitDelta() => pcInput.GetOrbitDelta() + touchInput.GetOrbitDelta();
        public float   GetZoomDelta()  => pcInput.GetZoomDelta()  + touchInput.GetZoomDelta();
        public Vector2 GetPanDelta()   => pcInput.GetPanDelta()   + touchInput.GetPanDelta();
    }
}

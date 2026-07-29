using UnityEngine;

namespace SimFusion.Anatomy.CameraControl
{
    /// <summary>
    /// Exposes ResetView() for a UI button.
    /// Delegates to OrbitCameraController — the existing smoothing handles the animation.
    /// </summary>
    public class CameraResetService : MonoBehaviour
    {
        [SerializeField] private OrbitCameraController orbitCamera;

        public void ResetView()
        {
            if (orbitCamera == null) return;
            orbitCamera.ResetTargetsToDefaults();
        }
    }
}

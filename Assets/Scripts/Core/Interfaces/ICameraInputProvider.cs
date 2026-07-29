using UnityEngine;

namespace SimFusion.Anatomy.Core
{
    /// Normalized camera input, implemented separately for PC and touch.
    public interface ICameraInputProvider
    {
        Vector2 GetOrbitDelta();
        float GetZoomDelta();
        Vector2 GetPanDelta();
    }
}

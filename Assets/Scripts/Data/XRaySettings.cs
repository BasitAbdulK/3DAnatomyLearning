using UnityEngine;

namespace SimFusion.Anatomy.Data
{
    /// Tunable parameters for X-Ray / transparency mode.
    [CreateAssetMenu(menuName = "Anatomy/XRay Settings", fileName = "XRaySettings")]
    public class XRaySettings : ScriptableObject
    {
        [Tooltip("Shared transparent URP Unlit material applied to all non-selected parts during X-Ray mode. Surface Type must be set to Transparent.")]
        [SerializeField] private Material xrayMaterial;

        public Material XRayMaterial => xrayMaterial;
    }
}

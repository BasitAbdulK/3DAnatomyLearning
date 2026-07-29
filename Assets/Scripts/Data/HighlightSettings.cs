using UnityEngine;

namespace SimFusion.Anatomy.Data
{
    [CreateAssetMenu(menuName = "Anatomy/Highlight Settings", fileName = "HighlightSettings")]
    public class HighlightSettings : ScriptableObject
    {
        [Header("Color")]
        [Tooltip("HDR tint applied to the selected part via MaterialPropertyBlock (_BaseColor).")]
        [ColorUsage(showAlpha: false, hdr: true)]
        [SerializeField] private Color highlightColor = new Color(1f, 0.65f, 0f);

        [Tooltip("Multiplier on top of highlightColor. " +
                 "Values above 1 push the colour into HDR range for visible brightness on URP Lit.")]
        [SerializeField, Range(1f, 5f)] private float highlightIntensity = 1.8f;

        [Header("Transition")]
        [Tooltip("Duration in seconds for highlight fade in/out. " +
                 "Set to 0 for instant. Used by future smooth-fade implementation.")]
        [SerializeField, Min(0f)] private float transitionDuration = 0.12f;

        public Color HighlightColor      => highlightColor;
        public float HighlightIntensity  => highlightIntensity;
        public float TransitionDuration  => transitionDuration;

        /// <summary>highlightColor pre-multiplied by intensity, ready to pass to _BaseColor.</summary>
        public Color FinalColor => new Color(
            highlightColor.r * highlightIntensity,
            highlightColor.g * highlightIntensity,
            highlightColor.b * highlightIntensity,
            1f);
    }
}

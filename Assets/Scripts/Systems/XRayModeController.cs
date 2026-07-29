using SimFusion.Anatomy.Data;
using SimFusion.Anatomy.Interaction;
using UnityEngine;

namespace SimFusion.Anatomy.Systems
{
    /// Makes all non-selected parts semi-transparent while X-Ray mode is active.
    /// Uses sharedMaterial swapping so no material instances are ever created.
    public class XRayModeController : MonoBehaviour
    {
        [Tooltip("Root of the body model hierarchy. Searched once on Start for PartSelectables.")]
        [SerializeField] private Transform modelRoot;
        [SerializeField] private SelectionManager selectionManager;
        [SerializeField] private XRaySettings settings;

        public bool IsXRayActive { get; private set; }

        private PartSelectable[] _parts;
        private Material[]       _originalMaterials;

        private void Start()
        {
            CachePartsAndMaterials();
        }

        private void OnEnable()
        {
            selectionManager.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            selectionManager.SelectionChanged -= OnSelectionChanged;
        }

        // Called by the X-Ray Button's onClick UnityEvent.
        public void Toggle()
        {
            SetXRayEnabled(!IsXRayActive);
        }

        public void SetXRayEnabled(bool enabled)
        {
            IsXRayActive = enabled;

            if (enabled)
                ApplyXRay(selectionManager.CurrentSelection);
            else
                RestoreAll();
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private void CachePartsAndMaterials()
        {
            _parts = modelRoot != null
                ? modelRoot.GetComponentsInChildren<PartSelectable>(includeInactive: true)
                : FindObjectsByType<PartSelectable>(FindObjectsSortMode.None);

            _originalMaterials = new Material[_parts.Length];
            for (int i = 0; i < _parts.Length; i++)
            {
                Renderer r = _parts[i].CachedRenderer;
                _originalMaterials[i] = r != null ? r.sharedMaterial : null;
            }
        }

        private void ApplyXRay(PartSelectable selected)
        {
            if (settings == null || settings.XRayMaterial == null)
                return;

            for (int i = 0; i < _parts.Length; i++)
            {
                PartSelectable part = _parts[i];
                if (part == null || part.CachedRenderer == null)
                    continue;

                bool isSelected = part == selected;
                part.CachedRenderer.sharedMaterial = isSelected
                    ? _originalMaterials[i]
                    : settings.XRayMaterial;
            }
        }

        private void RestoreAll()
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                PartSelectable part = _parts[i];
                if (part != null && part.CachedRenderer != null)
                    part.CachedRenderer.sharedMaterial = _originalMaterials[i];
            }
        }

        private void OnSelectionChanged(PartSelectable previous, PartSelectable current)
        {
            if (!IsXRayActive)
                return;

            // Restore the newly selected part to its original material so
            // HighlightService can apply its MPB on top of the correct base material.
            if (current != null && IndexOf(current) is int ci && ci >= 0)
                current.CachedRenderer.sharedMaterial = _originalMaterials[ci];

            // Make the previously selected part transparent again.
            if (previous != null && settings?.XRayMaterial != null && IndexOf(previous) is int pi && pi >= 0)
                previous.CachedRenderer.sharedMaterial = settings.XRayMaterial;
        }

        private int IndexOf(PartSelectable part)
        {
            for (int i = 0; i < _parts.Length; i++)
                if (_parts[i] == part) return i;
            return -1;
        }
    }
}

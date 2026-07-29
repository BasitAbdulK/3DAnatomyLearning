using SimFusion.Anatomy.Interaction;
using UnityEngine;

namespace SimFusion.Anatomy.Systems
{
    /// Drives cross-section visibility by hiding parts whose bounds center is above a
    /// horizontal plane. Uses GameObject.SetActive — no shader or mesh changes required.
    public class SectionPlaneController : MonoBehaviour
    {
        [Tooltip("Root of the body model hierarchy. Searched once on Start for PartSelectables.")]
        [SerializeField] private Transform modelRoot;
        [SerializeField] private SelectionManager selectionManager;

        public bool IsActive { get; private set; }

        private PartSelectable[] _parts;
        private bool[]           _activeBeforeSection; // per-part state at the moment section was enabled
        private float            _planeY;
        private float            _modelMinY;
        private float            _modelMaxY;

        private void Start()
        {
            CacheParts();
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public void Toggle() => SetSectionActive(!IsActive);

        public void SetSectionActive(bool active)
        {
            if (active == IsActive)
                return;

            IsActive = active;

            if (active)
            {
                // Snapshot which parts are currently active before we change anything.
                for (int i = 0; i < _parts.Length; i++)
                    _activeBeforeSection[i] = _parts[i] != null && _parts[i].gameObject.activeSelf;

                ApplyPlane();
            }
            else
            {
                RestoreAll();
            }
        }

        /// <param name="t">0 = plane at model top (nothing cut, all visible).
        ///                  1 = plane at model bottom (all hidden).</param>
        public void SetPlaneNormalized(float t)
        {
            // Subtracting a small margin from the bottom target ensures that at t=1
            // every part's center is strictly above _planeY and therefore hidden.
            const float bottomMargin = 0.01f;
            _planeY = Mathf.Lerp(_modelMinY - bottomMargin, _modelMaxY + bottomMargin, t);
            if (IsActive)
                ApplyPlane();
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private void CacheParts()
        {
            _parts = modelRoot != null
                ? modelRoot.GetComponentsInChildren<PartSelectable>(includeInactive: true)
                : FindObjectsByType<PartSelectable>(FindObjectsSortMode.None);

            _activeBeforeSection = new bool[_parts.Length];
            ComputeModelBounds();
            // Default plane below model → no parts hidden when first enabled.
            _planeY = _modelMinY - 0.01f;
        }

        private void ComputeModelBounds()
        {
            _modelMinY = float.MaxValue;
            _modelMaxY = float.MinValue;

            foreach (PartSelectable part in _parts)
            {
                if (part == null) continue;
                float y = part.CachedRenderer != null
                    ? part.CachedRenderer.bounds.center.y
                    : part.transform.position.y;
                if (y < _modelMinY) _modelMinY = y;
                if (y > _modelMaxY) _modelMaxY = y;
            }

            // Guard against a model with a single part or no parts.
            if (_modelMinY >= _modelMaxY)
            {
                _modelMinY -= 0.5f;
                _modelMaxY += 0.5f;
            }
        }

        private void ApplyPlane()
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                PartSelectable part = _parts[i];
                if (part == null) continue;

                // A part that was already hidden (e.g. by BodySystemManager) stays hidden.
                if (!_activeBeforeSection[i])
                {
                    part.gameObject.SetActive(false);
                    continue;
                }

                float centerY = part.CachedRenderer != null
                    ? part.CachedRenderer.bounds.center.y
                    : part.transform.position.y;

                bool aboveOrAtPlane = centerY >= _planeY;
                part.gameObject.SetActive(aboveOrAtPlane);
            }

            // If the selected part was just hidden, clear the selection so UI stays consistent.
            if (selectionManager != null
                && selectionManager.CurrentSelection != null
                && !selectionManager.CurrentSelection.gameObject.activeSelf)
            {
                selectionManager.ClearSelection();
            }
        }

        private void RestoreAll()
        {
            for (int i = 0; i < _parts.Length; i++)
                if (_parts[i] != null)
                    _parts[i].gameObject.SetActive(_activeBeforeSection[i]);
        }
    }
}

using SimFusion.Anatomy.Data;
using UnityEngine;

namespace SimFusion.Anatomy.Interaction
{

    /// Applies and removes highlight colours using MaterialPropertyBlock.
    /// Responsible only for visual feedback — knows nothing about input, UI, or camera.

    public class HighlightService : MonoBehaviour
    {
        [SerializeField] private SelectionManager  selectionManager;
        [SerializeField] private HighlightSettings settings;

        // Cached once at class load — Shader.PropertyToID never changes at runtime.
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        // Initialised in Awake — MaterialPropertyBlock cannot be constructed in a
        // MonoBehaviour field initializer (Unity's scripting constructor restriction).
        private MaterialPropertyBlock _mpb;

        // The part currently being highlighted (T: 0 → 1).
        private PartSelectable _incoming;
        private float          _incomingT;

        // The part currently being restored to original (T: was → 0).
        private PartSelectable _outgoing;
        private float          _outgoingT;

        // ── Lifecycle ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            selectionManager.SelectionChanged += OnSelectionChanged;
            selectionManager.SelectionCleared += OnSelectionCleared;
        }

        private void OnDisable()
        {
            selectionManager.SelectionChanged -= OnSelectionChanged;
            selectionManager.SelectionCleared -= OnSelectionCleared;
        }

        private void Update()
        {
            if (settings == null || settings.TransitionDuration <= 0f)
                return;

            float step = Time.deltaTime / settings.TransitionDuration;

            if (_incoming != null)
            {
                _incomingT = Mathf.MoveTowards(_incomingT, 1f, step);
                ApplyLerped(_incoming, _incomingT);
                if (_incomingT >= 1f)
                    _incoming = null;
            }

            if (_outgoing != null)
            {
                _outgoingT = Mathf.MoveTowards(_outgoingT, 0f, step);
                ApplyLerped(_outgoing, _outgoingT);
                if (_outgoingT <= 0f)
                {
                    SnapToOriginal(_outgoing);
                    _outgoing = null;
                }
            }
        }

        // ── Event handlers ─────────────────────────────────────────────────────────────────

        private void OnSelectionChanged(PartSelectable previous, PartSelectable current)
        {
            // A stale outgoing from a prior rapid change that isn't the new previous:
            // snap it instantly so no part is ever left in a partial highlight state.
            if (_outgoing != null && _outgoing != previous)
                SnapToOriginal(_outgoing);

            _outgoing = previous;
            // If the same part was interrupted mid-highlight, restore from where it was;
            // otherwise it was fully highlighted, so start the restore from 1.
            _outgoingT = (previous != null && previous == _incoming) ? _incomingT : 1f;

            _incoming  = current;
            _incomingT = 0f;

            if (settings == null || settings.TransitionDuration <= 0f)
            {
                if (_incoming != null) ApplyHighlight(_incoming);
                SnapToOriginal(_outgoing);
                _incoming = null;
                _outgoing = null;
            }
        }

        private void OnSelectionCleared()
        {
            // SelectionChanged(previous, null) fires right after this and owns the restore.
            // Guard only for instant mode: snap any incoming still mid-transition.
            if (settings != null && settings.TransitionDuration <= 0f && _incoming != null)
            {
                SnapToOriginal(_incoming);
                _incoming = null;
            }
        }

        // ── Highlight helpers ──────────────────────────────────────────────────────────────

        private void ApplyHighlight(PartSelectable part)
        {
            if (!IsValid(part)) return;
            SetColor(part.CachedRenderer, settings.FinalColor);
        }

        private void ApplyLerped(PartSelectable part, float t)
        {
            if (!IsValid(part)) return;
            // sharedMaterial is always the unmodified original — we never touch it.
            Color original = part.CachedRenderer.sharedMaterial.GetColor(BaseColorId);
            SetColor(part.CachedRenderer, Color.Lerp(original, settings.FinalColor, t));
        }

        private void SnapToOriginal(PartSelectable part)
        {
            if (!IsValid(part)) return;
            // Passing null clears the block entirely; the renderer falls back to the
            // shared material's own properties with no new material instance created.
            part.CachedRenderer.SetPropertyBlock(null);
        }

        private void SetColor(Renderer r, Color color)
        {
            _mpb.SetColor(BaseColorId, color);
            r.SetPropertyBlock(_mpb); // Unity copies values in — no reference retained
        }

        // Destroyed UnityEngine.Objects compare equal to null via operator overloading.
        private static bool IsValid(PartSelectable part)
            => part != null && part.CachedRenderer != null;
    }
}

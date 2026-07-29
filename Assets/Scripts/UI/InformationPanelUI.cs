using SimFusion.Anatomy.Data;
using SimFusion.Anatomy.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimFusion.Anatomy.UI
{
    /// <summary>
    /// Displays body part details in response to selection events.
    /// Subscribes only to SelectionManager — knows nothing about input, highlight, or camera.
    /// Handles missing BodyPartData fields gracefully without throwing exceptions.
    /// </summary>
    public class InformationPanelUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SelectionManager selectionManager;

        [Header("Panel Root")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Text Fields")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text systemText;

        [Header("Icon")]
        [SerializeField] private Image iconImage;

        [Header("Animation")]
        [Tooltip("Fade duration in seconds. Set to 0 for instant show/hide.")]
        [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

        // Fallbacks keep the panel readable when a ScriptableObject is partially configured.
        private const string FallbackName        = "Unknown Body Part";
        private const string FallbackDescription = "No description available.";
        private const string FallbackSystem      = "Unknown System";

        private float _targetAlpha;

        // ── Lifecycle ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            canvasGroup.alpha          = 0f;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
            _targetAlpha = 0f;
        }

        private void OnEnable()
        {
            selectionManager.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            selectionManager.SelectionChanged -= OnSelectionChanged;
        }

        private void Update()
        {
            if (fadeDuration <= 0f) return;
            if (Mathf.Approximately(canvasGroup.alpha, _targetAlpha)) return;

            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha, _targetAlpha, Time.deltaTime / fadeDuration);

            SyncInteractivity();
        }

        // ── Event handler ─────────────────────────────────────────────────────────────────

        private void OnSelectionChanged(PartSelectable previous, PartSelectable current)
        {
            if (current != null)
            {
                Populate(current.Data);
                SetVisible(true);
            }
            else
            {
                SetVisible(false);
            }
        }

        // ── Display ───────────────────────────────────────────────────────────────────────

        private void Populate(BodyPartData data)
        {
            nameText.text = !string.IsNullOrWhiteSpace(data?.DisplayName)
                ? data.DisplayName
                : FallbackName;

            descriptionText.text = !string.IsNullOrWhiteSpace(data?.Description)
                ? data.Description
                : FallbackDescription;

            systemText.text = data?.System != null && !string.IsNullOrWhiteSpace(data.System.DisplayName)
                ? data.System.DisplayName
                : FallbackSystem;

            bool hasIcon = data?.Icon != null;
            iconImage.gameObject.SetActive(hasIcon);
            if (hasIcon)
                iconImage.sprite = data.Icon;
        }

        // ── Animation ─────────────────────────────────────────────────────────────────────

        private void SetVisible(bool visible)
        {
            _targetAlpha = visible ? 1f : 0f;

            if (fadeDuration <= 0f)
            {
                canvasGroup.alpha = _targetAlpha;
                SyncInteractivity();
            }
        }

        private void SyncInteractivity()
        {
            canvasGroup.interactable   = canvasGroup.alpha >= 1f;
            canvasGroup.blocksRaycasts = canvasGroup.alpha > 0f;
        }
    }
}

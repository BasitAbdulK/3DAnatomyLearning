using SimFusion.Anatomy.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace SimFusion.Anatomy.UI
{
    /// UI panel for cross-section mode: a toggle enables/disables the plane,
    /// a slider controls the cut depth. Slider is hidden when section is off.
    public class SectionSliderView : MonoBehaviour
    {
        [SerializeField] private SectionPlaneController sectionController;
        [SerializeField] private Toggle                 sectionToggle;
        [SerializeField] private Slider                 sectionSlider;
        [SerializeField] private GameObject             sliderContainer;

        private bool _suppressCallback;

        private void OnEnable()
        {
            sectionToggle.onValueChanged.AddListener(OnToggleChanged);
            sectionSlider.onValueChanged.AddListener(OnSliderChanged);
            SyncSliderVisibility();
        }

        private void OnDisable()
        {
            sectionToggle.onValueChanged.RemoveListener(OnToggleChanged);
            sectionSlider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            if (_suppressCallback) return;
            sectionController.SetSectionActive(isOn);
            SyncSliderVisibility();
        }

        private void OnSliderChanged(float value)
        {
            if (_suppressCallback) return;
            sectionController.SetPlaneNormalized(value);
        }

        private void SyncSliderVisibility()
        {
            if (sliderContainer != null)
                sliderContainer.SetActive(sectionToggle.isOn);
        }
    }
}

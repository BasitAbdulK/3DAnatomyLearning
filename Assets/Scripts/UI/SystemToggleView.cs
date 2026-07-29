using System;
using SimFusion.Anatomy.Data;
using SimFusion.Anatomy.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimFusion.Anatomy.UI
{
    /// One toggle UI row bound to a single BodySystemData.
    /// Wires itself to BodySystemManager on Enable and raises ToggleChanged on interaction.
    [RequireComponent(typeof(Toggle))]
    public class SystemToggleView : MonoBehaviour
    {
        [SerializeField] private BodySystemData    system;
        [SerializeField] private BodySystemManager systemManager;
        [SerializeField] private TMP_Text          label;

        public event Action<BodySystemData, bool> ToggleChanged;

        private Toggle _toggle;
        private bool   _suppressCallback;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnToggleChanged);

            // Sync label text from the SO.
            if (label != null && system != null)
                label.text = system.DisplayName;

            // Reflect the manager's current state (e.g. after a scene reload).
            if (systemManager != null && system != null)
            {
                _suppressCallback = true;
                _toggle.isOn      = systemManager.IsSystemVisible(system);
                _suppressCallback = false;
            }

            if (systemManager != null)
                systemManager.SystemVisibilityChanged += OnManagerVisibilityChanged;
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);

            if (systemManager != null)
                systemManager.SystemVisibilityChanged -= OnManagerVisibilityChanged;
        }

        private void OnToggleChanged(bool isOn)
        {
            if (_suppressCallback)
                return;

            ToggleChanged?.Invoke(system, isOn);
            systemManager?.SetSystemVisible(system, isOn);
        }

        // Keep toggle visual in sync if another caller changes visibility (e.g. Save/Load later).
        private void OnManagerVisibilityChanged(BodySystemData changedSystem, bool visible)
        {
            if (changedSystem != system)
                return;

            _suppressCallback = true;
            _toggle.isOn      = visible;
            _suppressCallback = false;
        }
    }
}

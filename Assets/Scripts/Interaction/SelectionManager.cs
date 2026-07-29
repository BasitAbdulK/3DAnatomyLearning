using System;
using UnityEngine;

namespace SimFusion.Anatomy.Interaction
{
    /// <summary>
    /// Single source of truth for the current part selection.
    /// Subscribes to InputRaycaster events and maintains CurrentSelection.
    /// Raises SelectionChanged and SelectionCleared; performs no visual updates.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private InputRaycaster inputRaycaster;

        /// <summary>Fires whenever the selection changes, including being cleared.
        /// previous is null on first selection; current is null when selection is cleared.</summary>
        public event Action<PartSelectable, PartSelectable> SelectionChanged;

        /// <summary>Fires when the selection is explicitly cleared (click on empty space).</summary>
        public event Action SelectionCleared;

        public PartSelectable CurrentSelection { get; private set; }

        private void OnEnable()
        {
            inputRaycaster.PartClicked       += OnPartClicked;
            inputRaycaster.EmptySpaceClicked += OnEmptySpaceClicked;
        }

        private void OnDisable()
        {
            inputRaycaster.PartClicked       -= OnPartClicked;
            inputRaycaster.EmptySpaceClicked -= OnEmptySpaceClicked;
        }

        private void OnPartClicked(PartSelectable clicked)
        {
            if (clicked == CurrentSelection)
                return;

            PartSelectable previous = CurrentSelection;
            previous?.Deselect();

            CurrentSelection = clicked;
            CurrentSelection.Select();

            SelectionChanged?.Invoke(previous, CurrentSelection);
        }

        /// Programmatically clears the selection. Called by systems (e.g. SectionPlaneController)
        /// that hide the currently selected part and need to keep UI state consistent.
        public void ClearSelection()
        {
            OnEmptySpaceClicked();
        }

        private void OnEmptySpaceClicked()
        {
            if (CurrentSelection == null)
                return;

            PartSelectable previous = CurrentSelection;
            previous.Deselect();
            CurrentSelection = null;

            SelectionCleared?.Invoke();
            SelectionChanged?.Invoke(previous, null);
        }
    }
}

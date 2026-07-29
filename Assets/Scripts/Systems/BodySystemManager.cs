using System;
using System.Collections.Generic;
using SimFusion.Anatomy.Data;
using SimFusion.Anatomy.Interaction;
using UnityEngine;

namespace SimFusion.Anatomy.Systems
{
    /// Tracks all PartSelectables under modelRoot and shows/hides them by BodySystemData.
    /// Data-driven: adding a new system requires only a new BodySystemData asset — no code changes here.
    public class BodySystemManager : MonoBehaviour
    {
        [Tooltip("Root of the body model hierarchy. Only its children are searched for PartSelectables.")]
        [SerializeField] private Transform modelRoot;

        public event Action<BodySystemData, bool> SystemVisibilityChanged;

        private readonly Dictionary<BodySystemData, List<PartSelectable>> _partsBySystem
            = new Dictionary<BodySystemData, List<PartSelectable>>();

        private readonly Dictionary<BodySystemData, bool> _visibilityBySystem
            = new Dictionary<BodySystemData, bool>();

        private void Start()
        {
            BuildPartRegistry();
        }

        /// Collects every PartSelectable under modelRoot and groups it by its BodySystemData.
        /// includeInactive: true so parts that start hidden are still tracked.
        public void BuildPartRegistry()
        {
            _partsBySystem.Clear();
            _visibilityBySystem.Clear();

            PartSelectable[] allParts = modelRoot != null
                ? modelRoot.GetComponentsInChildren<PartSelectable>(includeInactive: true)
                : FindObjectsByType<PartSelectable>(FindObjectsSortMode.None);

            foreach (PartSelectable part in allParts)
            {
                BodySystemData system = part.Data != null ? part.Data.System : null;
                if (system == null)
                    continue;

                if (!_partsBySystem.ContainsKey(system))
                {
                    _partsBySystem[system]      = new List<PartSelectable>();
                    _visibilityBySystem[system] = true;
                }
                _partsBySystem[system].Add(part);
            }
        }

        /// Shows or hides all parts belonging to <paramref name="system"/>.
        public void SetSystemVisible(BodySystemData system, bool visible)
        {
            if (system == null)
                return;

            _visibilityBySystem[system] = visible;

            if (!_partsBySystem.TryGetValue(system, out List<PartSelectable> parts))
                return;

            foreach (PartSelectable part in parts)
            {
                if (part != null)
                    part.gameObject.SetActive(visible);
            }

            SystemVisibilityChanged?.Invoke(system, visible);
        }

        /// Returns the current visibility state for a system, defaulting to true.
        public bool IsSystemVisible(BodySystemData system)
        {
            if (system == null)
                return true;

            return !_visibilityBySystem.TryGetValue(system, out bool v) || v;
        }
    }
}

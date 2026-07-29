using UnityEngine;
using SimFusion.Anatomy.Core;
using SimFusion.Anatomy.Data;

namespace SimFusion.Anatomy.Interaction
{
    /// Marker component that makes a GameObject selectable.
    /// Holds the BodyPartData reference and caches the Renderer for HighlightService.
    /// Contains no visual or selection logic.
    [RequireComponent(typeof(Collider))]
    public class PartSelectable : MonoBehaviour, ISelectable
    {
        [SerializeField] private BodyPartData data;

        public BodyPartData Data           => data;
        public Renderer     CachedRenderer { get; private set; }
        public bool         IsSelected     { get; private set; }

        private void Awake()
        {
            CachedRenderer = GetComponent<Renderer>();
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }
    }
}

using UnityEngine;

namespace SimFusion.Anatomy.Data
{
    /// Static content for a single selectable body part (name + description).
    [CreateAssetMenu(menuName = "Anatomy/Body Part Data", fileName = "NewBodyPartData")]
    public class BodyPartData : ScriptableObject
    {
        [SerializeField] private string partId;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private BodySystemData system;

        [Header("Panel")]
        [Tooltip("Optional icon shown in the information panel. Leave empty to hide the image.")]
        [SerializeField] private Sprite icon;

        public string        PartId      => partId;
        public string        DisplayName => displayName;
        public string        Description => description;
        public BodySystemData System     => system;
        public Sprite        Icon        => icon;
    }
}

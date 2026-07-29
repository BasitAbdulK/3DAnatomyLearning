using UnityEngine;

namespace SimFusion.Anatomy.Data
{
    /// Identifies a logical group of parts (Skeleton, Muscles, Organs...)
    [CreateAssetMenu(menuName = "Anatomy/Body System Data", fileName = "NewBodySystemData")]
    public class BodySystemData : ScriptableObject
    {
        [SerializeField] private string systemId;
        [SerializeField] private string displayName;

        public string SystemId => systemId;
        public string DisplayName => displayName;
    }
}

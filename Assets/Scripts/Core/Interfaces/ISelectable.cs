namespace SimFusion.Anatomy.Core
{
    /// Anything that can be selected/deselected via the interaction system.
    public interface ISelectable
    {
        void Select();
        void Deselect();
    }
}

namespace Hubris
{
    /// <summary>
    /// Interface for Entities which can be selected by the player
    /// </summary>
    interface ISelectable
    {
        void OnSelect();
        void OnDeselect();
    }
}

namespace Hubris
{
    /// <summary>
    /// Interface for objects that count things
    /// </summary>
    interface ICounter
    {
        bool Inc(int nAmt);     // Increment method
        bool Dec(int nAmt);     // Decrement method
    }
}

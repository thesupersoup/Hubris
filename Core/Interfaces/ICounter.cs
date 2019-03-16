namespace Hubris
{
    /// <summary>
    /// Interface for Entities that require increment and decrement functionality
    /// </summary>
    interface ICounter
    {
        bool Inc(int nAmt);   // Increment method
        bool Dec(int nAmt);   // Decrement method
    }
}

namespace Hubris
{
    /// <summary>
    /// Types of damage that can be dealt or received by an IDamagable
    /// </summary>
    public enum DmgType
    {
        BASE = 0,   // HP/Armor
        STAMINA,    // Stamina only
        // POISON,     // HP DoT
        ARMOR,      // Armor only
        // ACID,       // HP/Armor DoT
        // TRANQ,      // Stamina DoT
        NUM_TYPES   // Handy enum length hack
    }
}

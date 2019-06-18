namespace Hubris
{
    /// <summary>
    /// Interface for branches of an AI behavior tree
    /// </summary>
    public interface IBhvBranch
    {
        void ChangeBranch(Npc a, IBhvBranch b);
        void Invoke(Npc a);
    }
}

namespace Hubris
{
	/// <summary>
	/// Interface for the root objects in Behavior Trees
	/// </summary>
	public interface IBhvTree
	{
		void ChangeBranch( IBhvBranch b );
		void Invoke( Npc a );
	}
}

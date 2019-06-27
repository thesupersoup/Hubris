namespace Hubris
{
	/// <summary>
	/// Interface for the root objects in Behavior Trees
	/// </summary>
	public interface IBhvTree
	{
		BhvStatus Status { get; }
		void SetStatus( BhvStatus s );
		void ChangeBranch( IBhvNode n, Npc a );
		void Invoke( Npc a );
	}
}

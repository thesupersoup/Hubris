namespace Hubris
{
	/// <summary>
	/// Interface for branches of an AI behavior tree
	/// </summary>
	public interface IBhvBranch
	{
		BhvStatus Status { get; }
		void Invoke( BhvTree b, Npc a );
		void SetStatus( BhvStatus s );
	}
}

namespace Hubris
{
	/// <summary>
	/// Interface for nodes of an AI behavior tree
	/// </summary>
	public interface IBhvNode
	{
		BhvStatus Invoke( Npc a, BhvTree b );
	}
}

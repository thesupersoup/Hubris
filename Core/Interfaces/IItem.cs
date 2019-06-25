namespace Hubris
{
	/// <summary>
	/// Interface for Items in the Inventory which have primary and secondary functions when equipped
	/// </summary>
	public interface IItem
	{
		void Interact0();
		void Interact1();
	}
}

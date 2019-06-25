namespace Hubris
{
	/// <summary>
	/// Allows you to activate and deactivate Hubris functionality without enabling or disabling the underlying GameObject
	/// </summary>
	public interface IActivatable
	{
		void Activate();
		void Deactivate();
	}
}
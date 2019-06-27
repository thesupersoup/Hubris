namespace Hubris
{
	public interface IBhvDecorator
	{
		void SetHandle( BhvDecorator.Child c );
		BhvStatus DecorateResult( BhvStatus s );
	}
}

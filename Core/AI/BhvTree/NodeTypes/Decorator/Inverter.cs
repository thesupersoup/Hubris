namespace Hubris
{
	/// <summary>
	/// Inverter node for behavior tree, which inverts a status result
	/// </summary>
	public class Inverter : BhvDecorator
	{
		public Inverter( Child c ) : base( c ) { }

		public override BhvStatus DecorateResult( BhvStatus s )
		{
			BhvStatus result;

			switch(s)
			{
				case BhvStatus.FAILURE:
					result = BhvStatus.SUCCESS;
					break;
				case BhvStatus.SUCCESS:
					result = BhvStatus.FAILURE;
					break;
				default:
					result = BhvStatus.RUNNING;
					break;
			}

			return result;
		}
	}
}

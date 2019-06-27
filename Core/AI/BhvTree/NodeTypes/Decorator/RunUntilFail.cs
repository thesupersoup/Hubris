namespace Hubris
{
	public class RunUntilFail : BhvDecorator
	{
		public RunUntilFail( Child c ) : base( c ) { }

		public override BhvStatus DecorateResult( BhvStatus s )
		{
			if ( s == BhvStatus.FAILURE )
				return s;

			return BhvStatus.RUNNING;
		}
	}
}

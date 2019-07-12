using System;

namespace Hubris
{
	public abstract class BhvDecorator : IBhvNode, IBhvDecorator, IDisposable
	{
		public delegate BhvStatus Child( Npc a, BhvTree b );

		///--------------------------------------------------------------------
		/// BhvDecorator instance vars
		///--------------------------------------------------------------------

		private Child _handle = null;

		///--------------------------------------------------------------------
		/// BhvDecorator properties
		///--------------------------------------------------------------------

		public Child ChildHandle => _handle;

		///--------------------------------------------------------------------
		/// BhvDecorator methods
		///--------------------------------------------------------------------

		public BhvDecorator( Child c )
		{
			SetHandle( c );
		}

		public void SetHandle( Child c )
		{
			_handle = c;
		}

		public BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( ChildHandle == null )
				return BhvStatus.FAILURE;

			return DecorateResult( ChildHandle.Invoke( a, b ) );
		}

		public virtual BhvStatus DecorateResult( BhvStatus s )
		{
			return s;
			// Override with unique implementation in derived classes
		}

		#region Disposal
		public void Dispose()
		{
			_handle = null;
		}
		#endregion
	}
}

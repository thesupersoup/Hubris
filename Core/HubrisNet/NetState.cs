using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public abstract class NetState
	{
		protected abstract bool IsFriend(HubrisNet net);
		public abstract bool Connect(HubrisNet net);
		public abstract bool Disconnect(HubrisNet net);
		public abstract void Send(HubrisNet net);
		public abstract void Receive(HubrisNet net);
		public abstract void ChangeState(HubrisNet net, NetState state);
	}

	public class NetClosed : NetState
	{
		public static NetState Instance;

		/// <summary>
		/// This method mimics the C++ "friend" declaration, by checking if the instance passed matches the singleton instance of the "owning" class
		/// </summary>
		/// <param name="net"></param>
		/// <returns>Returns true if the parameter matches the singleton instance</returns>
		protected override bool IsFriend(HubrisNet net)
		{
			if (net == HubrisNet.Instance)
				return true;
			else
				return false;
		}

		public override bool Connect(HubrisNet net)
		{
			if (IsFriend(net))
			{
				return true;
			}
			else
				return false;
		}

		public override bool Disconnect(HubrisNet net)
		{
			if (IsFriend(net))
			{
				return true;
			}
			else
				return false;
		}

		public override void Receive(HubrisNet net)
		{
			if(IsFriend(net))
			{

			}
		}

		public override void Send(HubrisNet net)
		{
			if(IsFriend(net))
			{

			}
		}

		public override void ChangeState(HubrisNet net, NetState state)
		{
			if(IsFriend(net))
			{
				net.ChangeState(net, state);
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace Hubris
{
	/// <summary>
	/// RTS specific implementation of the GameManager class
	/// </summary>
	public class RTSGameManager : GameManager
	{
		///--------------------------------------------------------------------
		/// RTSGameManager instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private RTSUnit _select = null;

		///--------------------------------------------------------------------
		/// RTSGameManager properties
		///--------------------------------------------------------------------

		public bool CheckSelected
		{
			get { return (_select != null); }
		}

		public RTSUnit Selected
		{
			get { return _select; }
		}

		public static new RTSGameManager Instance
		{
			get { return (RTSGameManager)HubrisCore.Instance.GM; }
		}

		///--------------------------------------------------------------------
		/// RTSGameManager methods
		///--------------------------------------------------------------------

		public void SetSelected(RTSUnit nUnit)
		{
			_select = nUnit;
			_select.OnSelect();
		}

		public void MoveSelected(Vector3 nPos)
		{
			if (CheckSelected)
			{
				Selected.SetMovePos(nPos);
			}
		}

		public void Deselect()
		{
			if (CheckSelected)
			{
				_select.OnDeselect();
				_select = null;

				if (HubrisCore.Instance.Debug)
					UnityEngine.Debug.Log("Deselected the selected entity");
			}
		}
	}
}

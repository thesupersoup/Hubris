﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace Hubris
{
	/// <summary>
	/// Manages high level game logic, including monitoring Objectives
	/// </summary>
	[Serializable]
	public class GameManager : LogicalEntity
	{
		///--------------------------------------------------------------------
		/// GameManager instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		protected List<Objective> _objectives = null;

		[SerializeField]
		protected List<InterestPoint> _interestPts = null;

		[SerializeField]
		protected List<SpawnPoint> _spawnPts = null;

		///--------------------------------------------------------------------
		/// GameManager properties
		///--------------------------------------------------------------------

		public List<Objective> Objectives => _objectives;

		public List<InterestPoint> InterestPts => _interestPts;

		public List<SpawnPoint> SpawnPts => _spawnPts;

		public static GameManager Instance
		{
			get { return HubrisCore.Instance.GM; }
		}

		///--------------------------------------------------------------------
		/// GameManager methods
		///--------------------------------------------------------------------

		public override void Init()
		{
			if (HubrisCore.Instance == null || HubrisCore.Instance.GM != this)
			{
				return;
			}

			// Call UnsubTick() when cleaning up to prevent errors
			SubTick();

			_objectives = new List<Objective>();
			_interestPts = new List<InterestPoint>();
			_spawnPts = new List<SpawnPoint>();

			if (_interestPts != null && _interestPts.Count > 0)
			{
				for (int i = 0; i < _interestPts.Count; i++)
				{
					_objectives.Add(new Objective(_interestPts[i].InterestType, _interestPts[i]));
				}
			}
		}

		public void ClearInfo()
		{
			Objectives.Clear();
			InterestPts.Clear();
			SpawnPts.Clear();
		}

		public override void FixedTick()
		{

		}

		public override void Tick()
		{

		}

		public override void LateTick()
		{

		}

		public override void CleanUp(bool full = true)
		{
			if (!this._disposed)
			{
				if (full)
				{
					_act = false;
					_name = null;
				}

				// Need to Unsubscribe from Tick Actions when finished to prevent errors
				UnsubTick();

				_disposed = true;
			}
		}
	}
}

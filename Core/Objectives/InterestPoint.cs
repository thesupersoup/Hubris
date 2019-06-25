using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a point of interest in a level; a potential subject for an Objective
	/// </summary>
	public abstract class InterestPoint : Entity, IObservable<bool>
	{
		///--------------------------------------------------------------------
		/// Interest Point instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		protected List<IObserver<bool>> _observers = new List<IObserver<bool>>();

		[SerializeField]
		protected Objective.GoalType _iType;

		///--------------------------------------------------------------------
		/// Interest Point properties
		///--------------------------------------------------------------------

		public List<IObserver<bool>> Observers
		{
			get { return _observers; }
		}

		public Objective.GoalType InterestType
		{
			get { return _iType; }
			protected set { _iType = value; }
		}

		///--------------------------------------------------------------------
		/// Interest Point methods
		///--------------------------------------------------------------------

		public IDisposable Subscribe(IObserver<bool> observer)
		{
			if (!Observers.Contains(observer))
				Observers.Add(observer);
			return new Unsubscriber<IObserver<bool>>(Observers, observer);
		}

		public void NotifyObservers(bool status)
		{
			if (Observers != null && Observers.Count > 0)
			{
				for (int i = 0; i < Observers.Count; i++)
				{
					Observers[i].OnNext(status);
				}
			}
		}
	}
}

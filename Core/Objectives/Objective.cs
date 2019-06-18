using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Represents an objective to be completed for either players or NPCs
    /// </summary>
    [Serializable]
    public class Objective : IObserver<bool>
    {
        public enum GoalType
        {
            CREATE = 0,
            DESTINATION,
            MODIFY,
            USE,
            DESTROY
        }

        [SerializeField]
        private IObservable<bool> _subject = null;

        private IDisposable _unsubscriber = null;

        [SerializeField]
        private GoalType _goal;
        [SerializeField]
        private bool _completed = false;

        public IObservable<bool> Subject
        {
            get { return _subject; }
            protected set { _subject = value; }
        }

        public GoalType Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public bool Completed
        {
            get { return _completed; }
            protected set { _completed = value; }
        }

        public Objective(GoalType nType = GoalType.CREATE, IObservable<bool> nSub = null)
        {
            Goal = nType;
            Subject = nSub;
            _completed = false;

            // Attempt to subscribe to subject
            Subscribe();
        }

        public void Subscribe()
        {
            if (_subject != null)
                _unsubscriber = _subject.Subscribe(this);
        }

        public void OnCompleted()
        {
            // Unsubscribe from subject
            Unsubscribe();
        }

        public void OnError(Exception error)
        {
            LocalConsole.Instance.Log("Objective exception " + error.Message, true);
        }

        public void OnNext(bool value)
        {
            Completed = value;
            if (Completed)
                LocalConsole.Instance.Log("Objective completed!", true);
        }

        public void Unsubscribe()
        {
            if (_unsubscriber != null)
                _unsubscriber.Dispose();
        }

        ~Objective()
        {
            // Ensure that we unsubscribed in the event of unexpected disposal
            Unsubscribe();
        }
    }
}

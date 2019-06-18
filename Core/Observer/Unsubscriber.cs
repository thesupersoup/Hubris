using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Generic object to be passed from provider to observer for unsubscribing
    /// </summary>
    public class Unsubscriber<T> : IDisposable
    {
        private List<T> _list;
        private T _observer;

        // Start is called before the first frame update
        public Unsubscriber(List<T> nList, T nObs)
        {
            _list = nList;
            _observer = nObs;
        }

        public void Dispose()
        {
            if (!_list.Remove(_observer))
                LocalConsole.Instance.Log("Error unsubscribing observer via Unsubscriber", true);
        }
    }
}
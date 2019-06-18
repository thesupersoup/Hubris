using System;
using UnityEngine;

namespace Hubris
{
    [Serializable]
    public class Inventory
    {
        [SerializeField]
        private int _activeIndex;

        [SerializeField]
        private InvItem[] _slots = new InvItem[5];

        public int ActiveIndex
        {
            get { return _activeIndex; }
        }

        public InvItem[] Slots
        {
            get { return _slots; }
        }

        public Inventory()
        {
            _activeIndex = 0;
        }

        public void SetActiveSlot(int nIndex)
        {
            if(nIndex >= 0 && nIndex < _slots.Length)
            {
                _activeIndex = nIndex;
            }
        }
    }
}

using System;

namespace Hubris
{
    /// <summary>
    /// Node for MagQueue that wraps up a Magazine and ref to next node
    /// </summary>
    public class MagQueueNode : IDisposable
    {
        private Magazine _mag;
        private MagQueueNode _next = null;

        public Magazine Mag
        {
            get
            { return _mag; }
            set
            { _mag = value; }
        }

        public MagQueueNode Next
        {
            get
            { return _next; }
            set
            { _next = value; }
        }

        public MagQueueNode(Magazine nMag)
        {
            _mag = nMag;
        }

        public void Dispose()
        {
            _next = null;
        }
    }

    /// <summary>
    /// Represents all of the magazines for a player-held weapon in a Queue data structure. 
    /// </summary>
    public class MagQueue
    {
        // Dropped mags should be enqueued again if they have ammo remaining.

        // Might want an active count of mags without traversing queue
        private int _count;
        private MagQueueNode _head = null;
        private MagQueueNode _tail = null;

        private object _lock = new object();    // Threadsafe lock object

        public MagQueue()
        {
            _count = 0;
        }

        // Add a Magazine to the queue
        public void Enqueue(Magazine nMag)
        {
            lock (_lock)    // Lock for thread safety
            {
                MagQueueNode newNode = new MagQueueNode(nMag);
                if (_head == null)
                {
                    _count = 0; // Reset count, head was null
                    _head = newNode;
                    _tail = newNode;
                }
                else
                {
                    _count++;   // Increment
                    _tail.Next = newNode;
                    _tail = newNode;
                }
            }
        }

        // Remove a Magazine from the queue
        public Magazine Dequeue()
        {
            lock (_lock)    // Lock for thread safety
            {
                MagQueueNode tempNode = null;

                if(_head != null)
                {
                    _count--;   // Only decrement count if a mag was found to remove
                    tempNode = _head;
                    _head = _head.Next;
                }

                return tempNode?.Mag ?? new Magazine(0,0);  // Return empty mag if nothing found
            }
        }

        // Check if the queue has any nodes waiting
        public bool HasNodes()
        {
            return _head != null;
        }

        public int NumNodes()
        {
            return _count;
        }
    }
}
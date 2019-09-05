using System;

namespace Hubris
{
	/// <summary>
	/// Node for CmdQueue that wraps up a Command and associated data, with a ref to next node
	/// </summary>
	public sealed class CmdQueueNode : IDisposable
	{
		private Command _cmd = null;
		private InputState _state;
		private string _data = null;
		private CmdQueueNode _next = null;

		public Command Cmd
		{
			get
			{ return _cmd; }
			set
			{ _cmd = value; }
		}

		public InputState State
		{
			get 
			{ return _state; }
			set 
			{ _state = value; }
		}

		public string Data
		{
			get
			{ return _data; }
			set
			{ _data = value; }
		}

		public CmdQueueNode Next
		{
			get
			{ return _next; }
			set
			{ _next = value; }
		}

		public CmdQueueNode( Command nCmd, InputState nState, string nData )
		{
			Cmd = nCmd;
			State = nState;
			Data = nData;
		}

		public void Dispose()
		{
			Cmd = null;
			Data = null;
			Next = null;
		}
	}

	/// <summary>
	/// Implementation of Queue data structure for Commands
	/// </summary>
	public class CmdQueue
	{
		private CmdQueueNode _head = null;
		private CmdQueueNode _tail = null;

		private object _lock = new object();    // Threadsafe lock object

		/// <summary>
		/// Add an Command to the queue
		/// </summary>
		public void Enqueue( Command nCmd, InputState nState, string nData = null )
		{
			lock (_lock)    // Lock for thread safety
			{
				CmdQueueNode newNode = new CmdQueueNode(nCmd, nState, nData);
				if (_head == null)
				{
					_head = newNode;
					_tail = newNode;
				}
				else
				{
					_tail.Next = newNode;
					_tail = newNode;
				}
			}
		}

		/// <summary>
		/// Remove a Command from the queue
		/// </summary>
		public Command Dequeue(out InputState nState, out string nData)
		{
			lock (_lock)    // Lock for thread safety
			{
				CmdQueueNode tempNode = null;

				if (_head != null)
				{
					tempNode = _head;
					_head = _head.Next;
				}

				if (tempNode != null)
				{
					nState = tempNode.State;
					nData = tempNode.Data;
					return tempNode.Cmd;
				}
				else
				{
					nState = InputState.INVALID;
					nData = null;
					return Command.None;
				}
			}
		}

		// Check if the queue has any nodes waiting
		public bool HasNodes()
		{
			return _head != null;
		}

		// Check the length of the queue
		public int NumNodes()
		{
			int num = 0;
			CmdQueueNode tempNode = _head;

			while (tempNode != null)
			{
				num++;
				tempNode = tempNode.Next;
			}

			return num;
		}
	}
}

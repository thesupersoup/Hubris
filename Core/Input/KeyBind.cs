using UnityEngine;
using System;

namespace Hubris
{
    [Serializable]
    public class KeyBind : IComparable
    {
        ///--------------------------------------------------------------------
        /// KeyBind instance vars
        ///--------------------------------------------------------------------

        private KeyCode _key;
        private Command _cmd;

        ///--------------------------------------------------------------------
        /// KeyBind properties
        ///--------------------------------------------------------------------

        public KeyCode Key
        {
            get { return _key; }
        }

        public Command Cmd
        {
            get { return _cmd; }
        }

        ///--------------------------------------------------------------------
        /// KeyBind methods
        ///--------------------------------------------------------------------

        public KeyBind()
        {
            _key = KeyCode.None;
            _cmd = Command.None;
        }

        public KeyBind(KeyCode kcKey, Command cCmd)
        {
            _key = kcKey;
            _cmd = cCmd;
        }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            if (obj is KeyBind nKb)
            {
                return (this.Key.CompareTo(nKb.Key));
            }
            else
            {
                throw new ArgumentException("KeyBind CompareTo(): obj is not a KeyBind");
            }
        }
    }
}

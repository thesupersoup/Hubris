using UnityEngine;
using System;

namespace Hubris
{
    [Serializable]
    public class KeyBind
    {
        // KeyBind instance variables
        private KeyCode key;
        private Command cmd;

        // KeyBind methods
        public KeyBind()
        {
            key = KeyCode.None;
            cmd = Command.None;
        }

        public KeyBind(KeyCode kcKey, Command cCmd)
        {
            key = kcKey;
            cmd = cCmd;
        }
    }
}

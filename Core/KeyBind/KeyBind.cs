using UnityEngine;
using System;

[Serializable]
public class KeyBind : System.Object
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

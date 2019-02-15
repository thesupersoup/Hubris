using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    // Singleton instance, to be populated by the derived class
    private static Player _i = null;

    private static object _lock = new object();
    private static bool _disposing = false; // Check if we're in the process of disposing this singleton

    public static Player Instance
    {
        get
        {
            if (_disposing)
                return null;
            else
                return _i;
        }

        set
        {
            lock (_lock)
            {
                if (_i == null)
                    _i = value;
            }
        }
    }

    public enum PType : byte { FPS = 0, FL, NUM_TYPES };    // Player type, whether First Person, Free Look, or others

    // Player variables
    private PType _type = PType.FPS;

    // Player properties
    public byte Type
    {
        get { return (byte)_type; }
        protected set { if (value > (byte)PType.FPS && value < (byte)PType.NUM_TYPES) { _type = (PType)value; } else { _type = PType.FPS; } }
    }

    // Player methods
    public abstract void Move(InputManager.Axis ax, float val);
    public abstract void CamRot(InputManager.Axis ax, float val);
}

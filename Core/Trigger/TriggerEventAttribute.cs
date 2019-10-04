using UnityEngine;
using System.Collections;

namespace Hubris.Core.Triggers
{
#if UNITY_EDITOR
    [System.AttributeUsage(System.AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
    public class TriggerEventAttribute : System.Attribute
    {
        // Use this attribute to mark an event that a Trigger can listen to
    }
#endif
}
using UnityEngine;
using System.Collections;

namespace Hubris.Core.Triggers
{
#if UNITY_EDITOR
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TriggerMethodAttribute : System.Attribute
    {
        // Use this attribute to mark an even that a Trigger can invoke
        // Can be used both on both static and instance methods
    }
#endif
}

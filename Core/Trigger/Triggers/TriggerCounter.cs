using UnityEngine;
using System.Collections;

namespace Hubris.Core.Triggers
{
    public class TriggerCounter : MonoBehaviour
    {
        [TriggerEvent]
        public event System.Action OnGoalReached;

        public int Count;

        public int Goal;

        [TriggerMethod]
        public void Increment(int value)
        {
            Count += value;

            if (Count == Goal && OnGoalReached != null)
            {
                OnGoalReached();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Triggers/Objects/Counter")]
        static void CreateCounter()
        {
            GameObject ga = new GameObject("Counter", typeof(TriggerCounter));

            if (UnityEditor.Selection.activeTransform is Transform trans)
            {
                ga.transform.SetParent(trans);
                ga.transform.localPosition = Vector3.zero;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, @"\..\Hubris\Gizmos\counter");
        }
#endif
    }
}

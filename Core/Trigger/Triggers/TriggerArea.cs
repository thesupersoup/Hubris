using UnityEngine;
using System.Collections;
using Hubris;

namespace Hubris.Core.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class TriggerArea : MonoBehaviour
    {
        [TriggerEvent]
        public event System.Action OnPlayerEntered;

        [TriggerEvent]
        public event System.Action OnEnemyEntered;

        void OnTriggerEnter(Collider col)
        {
            if (col.GetComponent<HubrisPlayer>() != null)
            {
                OnPlayerEntered?.Invoke();
            }
            else if (col.GetComponent<LiveEntity>() != null)
            {
                OnEnemyEntered?.Invoke();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Triggers/Objects/Area")]
        static void CreateNew()
        {
            GameObject ga = new GameObject("Trigger Area", typeof(BoxCollider), typeof(TriggerArea));
            ga.GetComponent<BoxCollider>().isTrigger = true;

            if (UnityEditor.Selection.activeTransform is Transform selected)
            {
                ga.transform.SetParent(selected);
                ga.transform.localPosition = Vector3.zero;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, @"\..\Hubris\Gizmos\area");
        }
#endif
    }
}

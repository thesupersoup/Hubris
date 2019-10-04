using UnityEngine;
using System.Collections;
using Hubris.Core.Coroutines;

namespace Hubris.Core.Triggers
{
    public class TriggerTimer : MonoBehaviour
    {
        [TriggerEvent]
        public event System.Action OnDelayedActivate;

        [TriggerEvent]
        public event System.Action OnPeriodic;

        public bool StartOnPlay;

        public float DelayTime;

        void Start()
        {
            if (StartOnPlay)
            {
                StartDelayedActivate();
                StartPeriodical();
            }
        }

        [TriggerMethod]
        public void StartDelayedActivate()
        {
            if (OnDelayedActivate != null && DelayTime > 0)
            {
                delayedActivate = StartCoroutine(Wait());
            }
        }

        [TriggerMethod]
        public void StopDelayedActivate()
        {
            if (delayedActivate != null) StopCoroutine(delayedActivate);
        }

        Coroutine delayedActivate;

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(DelayTime);

            OnDelayedActivate?.Invoke();
        }

        void Awake()
        {
            b_Periodical = new CoroutineWrapper(B_Periodical());
        }

        [TriggerMethod]
        public void StartPeriodical()
        {
            if (!b_Periodical.IsRunning)
            {
                b_Periodical.Start();
            }
        }

        [TriggerMethod]
        public void StopPeriodical()
        {
            if (b_Periodical.IsRunning)
            {
                b_Periodical.Stop();
            }
        }

        public float PeriodicalTime;
        CoroutineWrapper b_Periodical;
        IEnumerable B_Periodical()
        {
            yield return null;

            while (OnPeriodic != null && PeriodicalTime > 0)
            {
                yield return new WaitForSeconds(PeriodicalTime);

                OnPeriodic();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Triggers/Objects/Timer")]
        static void CreateCounter()
        {
            GameObject ga = new GameObject("Timer", typeof(TriggerTimer));

            if (UnityEditor.Selection.activeTransform is Transform trans)
            {
                ga.transform.SetParent(trans);
                ga.transform.localPosition = Vector3.zero;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, @"\..\Hubris\Gizmos\timer");
        }
#endif
    }
}

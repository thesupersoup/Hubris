using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Hubris.Core.Triggers
{
    [System.Serializable]
    public class Trigger : MonoBehaviour
    {
        public TriggerEventData triggerData = new TriggerEventData();
        public List<TriggerInvokeData> callbackData = new List<TriggerInvokeData>();

        public bool onlyTriggerOnce = true;

        private readonly List<System.Action> callbacks = new List<System.Action>();

        void Start()
        {
            RegisterCallbacks();
            RegisterTrigger();
        }

        void RegisterCallbacks()
        {
            for (int callBackId = 0; callBackId < callbackData.Count; callBackId++)
            {
                if (callbackData[callBackId].triggerType == TriggerInvokeType.Instance)
                {
                    var component = callbackData[callBackId].targetObject.GetComponent(callbackData[callBackId].targetComponent);
                    var method = component.GetType().GetMethod(callbackData[callBackId].targetMethod);

                    List<object> args = new List<object>();
                    for (int argId = 0; argId < callbackData[callBackId].argTypes.Length; argId++)
                    {
                        var convertType = Assembly.GetAssembly(typeof(System.Int32)).GetTypes().ToList().Find(x => x.Name == callbackData[callBackId].argTypes[argId]);

                        args.Add(System.Convert.ChangeType(callbackData[callBackId].argValues[argId], convertType));
                    }

                    callbacks.Add(() => method.Invoke(component, args.ToArray()));
                }
                else
                {
                    var component = System.Type.GetType(callbackData[callBackId].targetComponent);
                    var method = component.GetMethod(callbackData[callBackId].targetMethod);

                    List<object> args = new List<object>();
                    for (int argId = 0; argId < callbackData[callBackId].argTypes.Length; argId++)
                    {
                        var convertType = Assembly.GetAssembly(typeof(System.Int32)).GetTypes().ToList().Find(x => x.Name == callbackData[callBackId].argTypes[argId]);

                        args.Add(System.Convert.ChangeType(callbackData[callBackId].argValues[argId], convertType));
                    }

                    callbacks.Add(() => method.Invoke(null, args.ToArray()));
                }
            }
        }

        void RegisterTrigger()
        {
            EventInfo evnt = null;
            object triggerComp = null;
            if (triggerData.targetObject != null)
            {
                triggerComp = triggerData.targetObject.GetComponent(triggerData.targetComponent);
                var evnts = triggerComp.GetType().GetEvents().Where(e => e.Name == triggerData.targetEvent).ToList();
                if (evnts.Count > 0)
                {
                    evnt = evnts[0];
                }
            }
            else
            {
                var typ = System.Type.GetType(triggerData.targetComponent);
                evnt = typ.GetEvents().Where(e => e.Name == triggerData.targetEvent).First();
            }

            if (evnt == null)
            {
                Debug.Log("Trigger event not found! " + name);
                return;
            }

            //get the event and its delegate type
            var t = evnt.EventHandlerType;
            var tMethod = t.GetMethod("Invoke");

            //get parameters specified by the event delegate
            var param = new List<ParameterExpression>();
            foreach (var item in tMethod.GetParameters())
            {
                param.Add(Expression.Parameter(item.ParameterType, ""));
            }

            //get delegate containing Activate method
            System.Action activateMethod = Activate;

            //need this for the call method to call on the correct instance
            var getThis = Expression.Constant(this);

            var block = Expression.Call(getThis, activateMethod.Method);

            var sig = Expression.Lambda(t, block, param);

            var eventDelegate = sig.Compile();

            evnt.AddEventHandler(triggerComp, eventDelegate);
        }

        private EventInfo eventInfo;
        private System.Delegate eventDelegate;

        void Activate()
        {
            if (onlyTriggerOnce)
            {
                if (eventInfo != null)
                {
                    eventInfo.RemoveEventHandler(triggerData.targetObject.GetComponent(triggerData.targetComponent), eventDelegate);
                }
            }

            for (int callbackId = 0; callbackId < callbacks.Count; callbackId++)
            {
                callbacks[callbackId]();
            }
        }

#if UNITY_EDITOR
        public List<bool> foldouts = new List<bool>();

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, @"\..\Hubris\Gizmos\trigger");
        }

        void OnDrawGizmosSelected()
        {
            if (triggerData.targetObject != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, triggerData.targetObject.transform.position);
            }

            Gizmos.color = Color.green;
            for (int callbackId = 0; callbackId < callbackData.Count; callbackId++)
            {
                if (callbackData[callbackId].targetObject != null)
                {
                    Gizmos.DrawLine(transform.position, callbackData[callbackId].targetObject.transform.position);
                }
            }
        }
#endif
    }
}

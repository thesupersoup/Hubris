using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
   
namespace Hubris.Core.Triggers
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerInspector : Editor
    {

        [MenuItem("Triggers/Create Trigger")]
        static void CreateNewTrigger()
        {
            GameObject ga = new GameObject("Trigger", typeof(Trigger));

        }

        public override void OnInspectorGUI()
        {
            var allTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            Trigger trigger = (Trigger)target;

            #region Trigger
            var prevTriggerType = trigger.triggerData.triggerType;
            trigger.triggerData.triggerType = (TriggerInvokeType)EditorGUILayout.EnumPopup("Trigger Type:", trigger.triggerData.triggerType);
            if (prevTriggerType != trigger.triggerData.triggerType)
            {
                trigger.triggerData.targetComponent = "";
                trigger.triggerData.targetEvent = "";
                trigger.triggerData.targetObject = null;
            }

            if (trigger.triggerData.triggerType == TriggerInvokeType.Instance)
            {//non-static events
                var prev = trigger.triggerData.targetObject;
                trigger.triggerData.targetObject = (GameObject)EditorGUILayout.ObjectField("Trigger Object:", trigger.triggerData.targetObject, typeof(GameObject), true);
                if (trigger.triggerData.targetObject != prev)
                {
                    trigger.triggerData.targetComponent = "";
                    trigger.triggerData.targetEvent = "";
                }
            }

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Trigger Event:", GUILayout.MaxWidth(EditorGUIUtility.labelWidth));
                if (GUILayout.Button(trigger.triggerData.targetComponent + "." + trigger.triggerData.targetEvent, GUILayout.ExpandWidth(true)))
                {
                    if (trigger.triggerData.triggerType == TriggerInvokeType.Instance)
                    { //object triggers
                        if (trigger.triggerData.targetObject != null)
                        {
                            GenericMenu triggerMenu = new GenericMenu();

                            var compTypes = trigger.triggerData.targetObject.GetComponents<Component>().Select(c => c.GetType());
                            foreach (var item in compTypes)
                            {
                                var evnts = item.GetEvents();
                                foreach (var evnt in evnts)
                                {
                                    if (!evnt.GetAddMethod().IsStatic && evnt.GetCustomAttributes(typeof(TriggerEventAttribute), true).Length > 0)
                                    {
                                        var localType = item;
                                        var localEvent = evnt;
                                        triggerMenu.AddItem(new GUIContent(localType.Name + "." + localEvent.Name), false, () => { trigger.triggerData.targetComponent = localType.Name; trigger.triggerData.targetEvent = localEvent.Name; });
                                    }
                                }
                            }

                            triggerMenu.ShowAsContext();
                        }
                    }
                    else
                    { //static triggers
                        GenericMenu triggerMenu = new GenericMenu();

                        List<KeyValuePair<Type, EventInfo>> evnts = new List<KeyValuePair<Type, EventInfo>>();

                        foreach (var t in allTypes)
                        {
                            var tEvnts = t.GetEvents().Where(e => e.GetAddMethod().IsStatic && e.GetCustomAttributes(typeof(TriggerEventAttribute), true).Length > 0);
                            foreach (var tEvnt in tEvnts)
                            {
                                evnts.Add(new KeyValuePair<Type, EventInfo>(t, tEvnt));
                            }
                        }

                        foreach (var evnt in evnts)
                        {
                            var localType = evnt.Key;
                            var localEvent = evnt.Value;
                            triggerMenu.AddItem(new GUIContent(localType.Name + "/" + localEvent.Name), false, () => { trigger.triggerData.targetComponent = localType.Name; trigger.triggerData.targetEvent = localEvent.Name; });
                        }


                        triggerMenu.ShowAsContext();
                    }
                }
            }

            trigger.onlyTriggerOnce = EditorGUILayout.Toggle("Only Once", trigger.onlyTriggerOnce);
            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region Callbacks
            if (GUILayout.Button("Add Callback"))
            {
                trigger.foldouts.Add(false);
                trigger.callbackData.Add(new TriggerInvokeData());
            }


            int toRemove = -1;
            for (int callbackId = 0; callbackId < trigger.callbackData.Count; callbackId++)
            {
                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    trigger.foldouts[callbackId] = EditorGUILayout.Foldout(trigger.foldouts[callbackId], trigger.callbackData[callbackId].targetMethod != "" ? trigger.callbackData[callbackId].targetComponent + "." + trigger.callbackData[callbackId].targetMethod : "-Empty-");
                    if (GUILayout.Button("Remove"))
                    {
                        toRemove = callbackId;
                    }
                }

                EditorGUI.indentLevel++;

                if (trigger.foldouts[callbackId])
                {
                    var prevCallbackType = trigger.callbackData[callbackId].triggerType;
                    trigger.callbackData[callbackId].triggerType = (TriggerInvokeType)EditorGUILayout.EnumPopup("Callback Type:", trigger.callbackData[callbackId].triggerType);
                    if (prevCallbackType != trigger.callbackData[callbackId].triggerType)
                    {
                        trigger.callbackData[callbackId].targetMethod = "";
                        trigger.callbackData[callbackId].targetComponent = "";
                        trigger.callbackData[callbackId].targetObject = null;
                    }

                    if (trigger.callbackData[callbackId].triggerType == TriggerInvokeType.Instance)
                    {
                        var prevCallbackObject = trigger.callbackData[callbackId].targetObject;
                        trigger.callbackData[callbackId].targetObject = (GameObject)EditorGUILayout.ObjectField("Target object:", trigger.callbackData[callbackId].targetObject, typeof(GameObject), true);
                        if (trigger.callbackData[callbackId].targetObject != prevCallbackObject)
                        {
                            trigger.callbackData[callbackId].targetMethod = "";
                            trigger.callbackData[callbackId].targetComponent = "";
                        }
                    }


                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Callback Method:", GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

                        //select method
                        if (GUILayout.Button(trigger.callbackData[callbackId].targetComponent + "." + trigger.callbackData[callbackId].targetMethod))
                        {
                            GenericMenu actionMenu = new GenericMenu();

                            if (trigger.callbackData[callbackId].triggerType == TriggerInvokeType.Instance && trigger.callbackData[callbackId].targetObject != null)
                            { //object callbacks

                                var compTypes = trigger.callbackData[callbackId].targetObject.GetComponents<Component>().Select(c => c.GetType());

                                foreach (var item in compTypes)
                                {
                                    foreach (var itemMethod in item.GetMethods().Where(x => !x.IsStatic && x.GetCustomAttributes(typeof(TriggerMethodAttribute), true).Count() > 0))
                                    {
                                        var args = itemMethod.GetParameters();
                                        string methodName = item.Name + "/" + itemMethod.Name + "(";
                                        if (args.Length > 0)
                                        {
                                            methodName += args[0];

                                            for (int i = 1; i < args.Length; i++)
                                            {
                                                methodName += ", " + args[i].ParameterType;
                                            }
                                        }
                                        methodName += ")";
                                        actionMenu.AddItem(new GUIContent(methodName), false, SetAction, new object[] { trigger.callbackData[callbackId], item.Name, itemMethod.Name, args.Length });
                                    }
                                }
                            }
                            else
                            { //static callbacks
                                foreach (var item in allTypes)
                                {
                                    foreach (var itemMethod in item.GetMethods().Where(x => x.IsStatic && x.GetCustomAttributes(typeof(TriggerMethodAttribute), true).Count() > 0))
                                    {
                                        var args = itemMethod.GetParameters();
                                        string methodName = item.Name + "/" + itemMethod.Name + "(";
                                        if (args.Length > 0)
                                        {
                                            methodName += args[0];

                                            for (int i = 1; i < args.Length; i++)
                                            {
                                                methodName += ", " + args[i].ParameterType;
                                            }
                                        }
                                        methodName += ")";
                                        actionMenu.AddItem(new GUIContent(methodName), false, SetAction, new object[] { trigger.callbackData[callbackId], item.Name, itemMethod.Name, args.Length });
                                    }
                                }
                            }

                            actionMenu.ShowAsContext();
                        }
                    }

                    //set method params
                    if (trigger.callbackData[callbackId].targetComponent != "" && trigger.callbackData[callbackId].targetMethod != "")
                    {
                        var typeInfo = allTypes.Find(x => x.Name == trigger.callbackData[callbackId].targetComponent);


                        if (typeInfo != null)
                        {
                            var methodInfo = typeInfo.GetMethod(trigger.callbackData[callbackId].targetMethod);

                            if (methodInfo != null)
                            {
                                var args = methodInfo.GetParameters();
                                for (int argId = 0; argId < args.Length; argId++)
                                {
                                    trigger.callbackData[callbackId].argTypes[argId] = args[argId].ParameterType.Name;
                                    trigger.callbackData[callbackId].argValues[argId] = DrawArgField(args[argId].Name, trigger.callbackData[callbackId].argValues[argId], args[argId].ParameterType);
                                }
                            }
                        }
                    }

                }

                EditorGUI.indentLevel--;
            }

            if (toRemove != -1)
            {
                trigger.foldouts.RemoveAt(toRemove);
                trigger.callbackData.RemoveAt(toRemove);
            }
            #endregion

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }


        void SetAction(object input)
        {
            object[] actionData = (object[])input;
            TriggerInvokeData targ = (TriggerInvokeData)actionData[0];
            int argLength = (int)actionData[3];

            targ.targetComponent = (string)actionData[1];
            targ.targetMethod = (string)actionData[2];

            targ.argTypes = new string[argLength];
            targ.argValues = new string[argLength];
        }

        string DrawArgField(string argName, string argValue, Type argType)
        {
            Color bg = GUI.backgroundColor;

            try
            {
                Convert.ChangeType(argValue, argType);
            }
            catch
            {
                GUI.backgroundColor = Color.red;
            }

            argValue = EditorGUILayout.TextField(argName + "(" + argType.Name + ")", argValue);

            GUI.backgroundColor = bg;

            return argValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hubris.Core.Triggers
{
    [System.Serializable]
    public class TriggerInvokeData
    {
        public TriggerInvokeType triggerType;
        public GameObject targetObject;
        public string targetComponent = "";
        public string targetMethod = "";
        public string[] argTypes;
        public string[] argValues;
    }
}

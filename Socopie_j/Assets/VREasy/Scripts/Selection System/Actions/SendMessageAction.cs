using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class SendMessageAction : VRAction
    {
        public GameObject messageReceiver;
        public string messageName;

        public Type parameterType;
        public string parameter = null;
        
        public override void Trigger()
        {
            if (messageReceiver != null)
            {
                messageReceiver.SendMessage(messageName,parameter);
            }
        }
        
    }
}
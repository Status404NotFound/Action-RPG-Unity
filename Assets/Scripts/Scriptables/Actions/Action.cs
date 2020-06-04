using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR.Scriptable
{
    [System.Serializable]
    public class Action
    {
        public ActionType actionType;
        public Object action_obj;
    }

    public enum ActionType
    {
        attack,block,spell,parry
    }
}
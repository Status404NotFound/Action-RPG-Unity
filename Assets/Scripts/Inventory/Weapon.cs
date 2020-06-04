using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FR.Scriptable;

namespace FR.Inventory
{
    [CreateAssetMenu(menuName ="Items/Weapon")]
    public class Weapon : Item
    {
        public StringVariable oh_idle;
        public StringVariable th_idle;
        public GameObject modelPrefab;
        public ActionHolder[] actions;

        public RightHandPosition rh_position;
        public LeftHandPosition lh_position;

        public ActionHolder GetActionHolder(InputType inp)
        {
            for (int i = 0; i<actions.Length; i++)
            {
                if (actions[i].input == inp)
                {
                    return actions[i];
                }
            }

            return null;
        }
        public Action GetAction(InputType inp)
        {
            ActionHolder ah = GetActionHolder(inp);
            if (ah == null)
                return null;
            return ah.action;
        }
    }

    [System.Serializable]
    public class ActionHolder
    {
        public InputType input;
        public Action action;
    }
}

public enum InputType
{
    rb,lb,rt,lt
}
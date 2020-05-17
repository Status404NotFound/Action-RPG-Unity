using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    public class Item : ScriptableObject
    {
        public ItemType type;
        public UI_Info ui_info;

        public Runtime runtime;

        public class Runtime
        {
            public bool equiped;
        }

        [System.Serializable]
        public class UI_Info
        {
            public string itemName;
            public string itemDescription;
            public string skillDescription;
            public Sprite icon;
        }
    }

    public enum ItemType
    {
        weapon,armor,consumable,spell
    }
}
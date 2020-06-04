using FR.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "References/Current Item")]
    public class CurItem : ScriptableObject
    {
        public int index;
        public List<Item> value = new List<Item>();

        public Item Get()
        {
            if (value.Count == 0)
                return null;

            if (index > value.Count - 1)
                index = 0;

            return value[index];
        }

        public void Add(Item i)
        {
            value.Add(i);
        }

        public void Clear()
        {
            index = 0;
            value.Clear();
        }
    }
}
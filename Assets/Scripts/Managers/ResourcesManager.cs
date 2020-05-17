using SA.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Managers
{
    [CreateAssetMenu(menuName ="Single Instances/Resources Manager")]
    public class ResourcesManager : ScriptableObject
    {
        public Inventory.Inventory inventory;
        public RuntimeReferences runtime;
        public void Init()
        {
            inventory.Init();
            runtime.Init();
        }

        public Item GetItem(string id)
        {
            return inventory.GetItem(id);
        }

        public Weapon GetWeapon(string id)
        {
            Item item = GetItem(id);
            return (Weapon)item;
        }

        public Armor GetArmor(string id)
        {
            Item item = GetItem(id);
            return (Armor)item;
        }

        public List<Item> GetAllItemsOfType(ItemType type)
        {
            List<Item> retVal = new List<Item>();
            for(int i = 0; i< inventory.all_items.Count; i++)
            {
                if(inventory.all_items[i].type == type)
                {
                    retVal.Add(inventory.all_items[i]);
                }
            }
            return retVal;
        }
    }
}
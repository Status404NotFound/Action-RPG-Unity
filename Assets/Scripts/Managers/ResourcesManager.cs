using FR.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR.Managers
{
    [CreateAssetMenu(menuName ="Single Instances/Resources Manager")]
    public class ResourcesManager : ScriptableObject
    {
        public Inventory.Inventory inventory;
        public RuntimeReferences runtime;
        public InventoryData playerInventory;

        public void Init()
        {
            inventory = Resources.Load("Inventory") as Inventory.Inventory;
            runtime = Resources.Load("RuntimeReferences") as RuntimeReferences;
            playerInventory = Resources.Load("PlayerInventory") as InventoryData;

            inventory.Init();
            runtime.Init();
        }

        public void CopyInventoryToData(InventoryData inv)
        {
            for(int i = 0; i < inv.data.Count; i++)
            {
                AddItemOnInventory(inv.data[i]);
            }
        }

        public void InitPlayerInventory()
        {
            playerInventory.data.Clear();
        }

        void AddItemOnInventory(string id)
        {
            Item i = GetItem(id);
            AddItemOnInventory(i);
        }

        void AddItemOnInventory(Item i)
        {
            Item newItem = Instantiate(i);
            playerInventory.data.Add(newItem);
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
                Item it = inventory.all_items[i];
                switch (type)
                {
                    case ItemType.weapon:
                        if (it is Weapon)
                            retVal.Add(it);
                        break;
                    case ItemType.armor:
                        if (it is Armor)
                            retVal.Add(it);
                        break;
                    case ItemType.consumable:
                        if (it is Consumable)
                            retVal.Add(it);
                        break;
                    case ItemType.spell:
                        break;
                    default:
                        break;
                }
            }
            return retVal;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FR.Inventory;

namespace FR
{
    public class Character : MonoBehaviour
    {
        public CharacterBody character;
        public InventoryData inventoryData;

        [System.Serializable]
        public class CharacterBody
        {
            //Armor
            public SkinnedMeshRenderer chestArmor;
            public SkinnedMeshRenderer hamlet;
            public SkinnedMeshRenderer legsArmor;
            public SkinnedMeshRenderer braces;

            //Body
            public SkinnedMeshRenderer body;
            public SkinnedMeshRenderer head;
            public SkinnedMeshRenderer hands;
            public SkinnedMeshRenderer legs;
        }

        public SkinnedMeshRenderer GetArmorPart(ArmorType type)
        {
            switch (type)
            {
                case ArmorType.chest:
                    return character.chestArmor;
                case ArmorType.hands:
                    return character.braces;
                case ArmorType.helmet:
                    return character.hamlet;
                case ArmorType.legs:
                    return character.legsArmor;
                default:
                    return null;
            }
        }

        public SkinnedMeshRenderer GetBodyPart(ArmorType type)
        {
            switch (type)
            {
                case ArmorType.chest:
                    return character.body;
                case ArmorType.hands:
                    return character.hands;
                case ArmorType.helmet:
                    return character.head;
                case ArmorType.legs:
                    return character.legs;
                default:
                    return null;
            }
        }

        public void InitArmor()
        {
            for (int i = 0; i < 4; i++)
            {
                SkinnedMeshRenderer r = GetArmorPart((ArmorType)i);
                SkinnedMeshRenderer b = GetBodyPart((ArmorType)i);
                b.enabled = true;
                r.enabled = false;
            }
        }

        public void LoadItemsFromData()
        {
            if (inventoryData == null)
                return;

            InitArmor();

            for (int i = 0; i < inventoryData.data.Count; i++)
            {
                WearItem(inventoryData.data[i]);
            }
        }

        public void WearItem(Item item)
        {
            if ((item is Armor) == false)
            {
                return;
            }

            Armor a = (Armor)item;
            SkinnedMeshRenderer m = GetArmorPart(a.armorType);
            m.sharedMesh = a.armorMesh;
            m.enabled = true;

            SkinnedMeshRenderer b = GetBodyPart(a.armorType);
            b.enabled = a.baseBodyEnabled;
            /*
            Material[] newMats = new Material[a.materials.Length];
            for (int i = 0; i < a.materials.Length; i++)
            {
                newMats[i] = a.materials[i];
            }
            */
            m.materials = a.materials;
        }

        public void UnwearItem(ArmorType t)
        {
            SkinnedMeshRenderer m = GetArmorPart(t);
            SkinnedMeshRenderer b = GetBodyPart(t);

            m.enabled = false;
            b.enabled = true;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName ="Items/Armor")]
    public class Armor : Item
    {
        public ArmorType armorType;
        public Mesh armorMesh;
        public Material[] materials;
        public bool baseBodyEnabled;

        public Armor()
        {
            type = ItemType.armor;
        }
    }
}
public enum ArmorType
{
    chest,legs,hands,helmet
}
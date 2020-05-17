using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class Consumable : Item
    {
        public Consumable()
        {
            type = ItemType.consumable;
        }
    }
}
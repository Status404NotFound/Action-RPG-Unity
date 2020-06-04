using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR.Inventory
{
    [CreateAssetMenu(menuName = "Items/Right Hand Position")]
    public class RightHandPosition : ScriptableObject
    {
        public Vector3 pos;
        public Vector3 eulers;
    }
}
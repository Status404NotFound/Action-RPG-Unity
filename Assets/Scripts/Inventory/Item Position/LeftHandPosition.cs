using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName ="Items/Left Hand Position")]
    public class LeftHandPosition : ScriptableObject
    {
        public Vector3 pos;
        public Vector3 eulers;
    }
}
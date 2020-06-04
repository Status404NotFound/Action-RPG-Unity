using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Variables/Transform")]
    public class TransformVariable : ScriptableObject
    {
        public Transform value;

        public void Set(Transform v)
        {
            value = v;
        }
    }
}
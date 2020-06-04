using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR.Inventory
{
    [CreateAssetMenu(menuName = "Single Instances/Runtime References")]
    public class RuntimeReferences : ScriptableObject
    {
        public List<RuntimeWeapon> runtimeWeapons = new List<RuntimeWeapon>();

        public void Init()
        {
            runtimeWeapons.Clear();
        }

        public void RegisterRW(RuntimeWeapon rw)
        {
            runtimeWeapons.Add(rw);
        }

        public void UnRegisterRW(RuntimeWeapon rw)
        {
            if (runtimeWeapons.Contains(rw))
            {
                if (rw.instance)
                    Destroy(rw.instance);
                runtimeWeapons.Remove(rw);
            }
        }
    }
}
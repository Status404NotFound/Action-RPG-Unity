using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FR.Inventory;

namespace FR.Managers
{
    public class SessionManager : MonoBehaviour
    {
        public ResourcesManager resourcesManager;
        public InventoryData easternSet;
        public InventoryData vikingSet;
        public int set;

        public GameEvent onPlayerUpdate;

        private void Awake()
        {
            resourcesManager = Resources.Load("ResourcesManager") as ResourcesManager;
            resourcesManager.Init();
            resourcesManager.InitPlayerInventory();
        }

        private void Start()
        {
            if (set > 0)
            {
                resourcesManager.CopyInventoryToData(easternSet);
            }
            else
            {
                resourcesManager.CopyInventoryToData(vikingSet);
            }

            onPlayerUpdate.Raise();
        }
    }
}
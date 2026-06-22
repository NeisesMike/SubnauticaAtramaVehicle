using HarmonyLib;
using System.Linq;
using UnityEngine;

// PURPOSE: Ensure drones always have a functional compass on the GUI
// VALUE: Moderate. Could pass this off to individual drones.

namespace VehicleFramework.Patches
{
	[HarmonyPatch(typeof(uGUI_InventoryTab))]
	class uGUI_InventoryTabPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(uGUI_InventoryTab.OnOpenPDA))]
        public static bool uGUI_InventoryTabOnOpenPDAPrefix(uGUI_InventoryTab __instance)
        {
            if (VehicleTypes.Drone.MountedDrone == null)
            {
                return true;
            }

            __instance.usedStorageGrids.Clear();
            __instance.storageLabelKey = null;

            IItemsContainer itemsContainer = VehicleTypes.Drone.MountedDrone.InnateStorages.First().Container.GetComponent<VehicleChildComponents.InnateStorageContainer>().Container;

            if (itemsContainer != null)
            {
                __instance.storageLabelKey = itemsContainer.label;
                if (itemsContainer is ItemsContainer)
                {
                    __instance.storage.Init(itemsContainer as ItemsContainer);
                    __instance.usedStorageGrids.Add(__instance.storage);
                }
            }

            if (string.IsNullOrEmpty(__instance.storageLabelKey))
            {
                __instance.storageLabelKey = "StorageLabel";
            }
            __instance.UpdateStorageLabelText();

            return false;

        }
	}
}

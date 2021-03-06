using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace VehicleFramework
{
    [HarmonyPatch(typeof(PDA))]
    public class PDAPatcher
    {
        /*
         * This patch ensures our QuickSlots display as expected when inside the ModVehicle but not piloting it.
         * That is, when piloting the ModVehicle, we should see the ModVehicle's modules.
         * When merely standing in the ModVehicle, we should see our own items: knife, flashlight, scanner, etc
         */
        [HarmonyPostfix]
        [HarmonyPatch("Close")]
        public static void ClosePostfix()
        {
            ModVehicle mv = Player.main.GetVehicle() as ModVehicle;
            if (mv != null && !mv.IsPlayerPiloting())
            {
                uGUI.main.quickSlots.SetTarget(null);
            }
        }
    }
}

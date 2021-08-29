﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace VehicleFramework
{
    [HarmonyPatch(typeof(uGUI_EquipmentSlot))]
    public class uGUI_EquipmentSlotPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetState")]
		public static bool SetStatePrefix(uGUI_EquipmentSlot.State newState, uGUI_EquipmentSlot __instance)
        {
            Logger.Log(__instance.name + ": Set State");
            if (!__instance.transform.name.Contains("Vehicle") || VehicleBuilder.areModulesReady)
            {
                Logger.Log("good input");
				return true;
            }
            Logger.Log("bad input");
			return false;
		}
	}
}
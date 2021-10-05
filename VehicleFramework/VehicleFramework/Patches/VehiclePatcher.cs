﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;

namespace VehicleFramework
{
    [HarmonyPatch(typeof(Vehicle))]
    public class VehiclePatcher
    {
        /*
         * This collection of patches generally ensures our ModVehicles behave like normal Vehicles.
         * Each will be commented if necessary
         */

        [HarmonyPrefix]
        [HarmonyPatch("OnHandHover")]
        public static bool OnHandHoverPrefix(Vehicle __instance)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv != null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnHandClick")]
        public static bool OnHandClickPrefix(Vehicle __instance, EnergyInterface ___energyInterface)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv != null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ApplyPhysicsMove")]
        private static bool ApplyPhysicsMovePrefix(Vehicle __instance, ref bool ___wasAboveWater, ref VehicleAccelerationModifier[] ___accelerationModifiers)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv != null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateEnergyRecharge")]
        public static bool UpdateEnergyRechargePrefix(Vehicle __instance)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv == null)
            {
                return true;
            }
            // TODO
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("LazyInitialize")]
        public static bool LazyInitializePrefix(Vehicle __instance, ref EnergyInterface ___energyInterface)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv == null)
            {
                return true;
            }

            ___energyInterface = __instance.gameObject.GetComponent<EnergyInterface>();
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetAllStorages")]
        public static void GetAllStoragesPostfix(Vehicle __instance, ref List<IItemsContainer> containers)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv == null)
            {
                return;
            }

            foreach(var tmp in ((ModVehicle)__instance).InnateStorages)
            {
                containers.Add(tmp.Container.GetComponent<InnateStorageContainer>().container);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("IsPowered")]
        public static void IsPoweredPostfix(Vehicle __instance, ref EnergyInterface ___energyInterface, ref bool __result)
        {
            ModVehicle mv = __instance as ModVehicle;
            if (mv == null)
            {
                return;
            }
            if(!mv.isPoweredOn)
            {
                __result = false;
            }
        }

        [HarmonyPatch("Update")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /* This is basically a prefix for Vehicle.Update,
             * but we choose to transpile instead,
             * so that our code may be considered "core."
             * That is, it will be skipped if any other Prefix returns false.
             * This is desirable to be as "alike" normal Vehicles as possible;
             * in particular, this ensures compatibility with FreeLook
             * We must control our ModVehicle rotation within the core Vehicle.Update code.
             */
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newCodes = new List<CodeInstruction>(codes.Count + 2);
            CodeInstruction myNOP = new CodeInstruction(OpCodes.Nop);
            for (int i = 0; i < codes.Count + 2; i++)
            {
                newCodes.Add(myNOP);
            }
            // push reference to vehicle
            // Call a static function which takes a vehicle and ControlsRotation if it's a ModVehicle
            newCodes[0] = new CodeInstruction(OpCodes.Ldarg_0);
            newCodes[1] = CodeInstruction.Call(typeof(ModVehicle), nameof(ModVehicle.MaybeControlRotation));
            for (int i = 0; i < codes.Count; i++)
            {
                newCodes[i+2] = codes[i];
            }
            return newCodes.AsEnumerable();
        }
    }
}

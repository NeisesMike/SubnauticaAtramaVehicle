﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Handlers;

namespace VehicleFramework.UpgradeModules
{
    public static partial class ModuleManager
    {
        public static TechType AddSelectableModule(List<Tuple<TechType, int>> inputRecipe, string classId, string displayName, string description, Action<ModVehicle, int> onSelected, float cooldown, float energyCost=0f, Atlas.Sprite icon=null, string tabName = "MVCM")
        {
            List<CraftData.Ingredient> recipe = inputRecipe.Select(x => new CraftData.Ingredient(x.Item1, x.Item2)).ToList();
            TechType tt = ModulePrepper.RegisterModuleGeneric(recipe, classId, displayName, description, QuickSlotType.Selectable, icon, tabName: tabName);
            bool WrappedOnSelected(ModVehicle mv, int slotID, TechType moduleTechType)
            {
                if (moduleTechType == tt)
                {
                    onSelected(mv, slotID);
                    return true;
                }
                return false;
            }
            ModulePrepper.upgradeOnUseActions.Add(new Tuple<Func<ModVehicle, int, TechType, bool>, float, float>(WrappedOnSelected, cooldown, energyCost));
            return tt;
        }
    }
}

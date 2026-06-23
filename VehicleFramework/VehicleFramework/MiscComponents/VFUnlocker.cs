using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using VehicleFramework.Admin;

namespace VehicleFramework.MiscComponents
{
    internal class VFUnlocker : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(UnlockEverythingMomentarily());
        }
        private IEnumerator UnlockEverythingMomentarily()
        {
            List<string> unlockedTechs = new();
            yield return new WaitForSeconds(5f);

            //unlock vehicles that should be unlocked
            var list = Admin.VehicleManager.vehicleTypes
                .Where(v => !KnownTech.Contains(v.mv.TechType))
                .Where(v => v.mv.UnlockedWith == TechType.None || KnownTech.Contains(v.mv.UnlockedWith))
                .ToList();
            list.ForEach(v => KnownTech.Add(v.techType, true));

            //unlock upgrades that should be unlocked
            foreach (var uppy in Admin.UpgradeRegistrar.RegisteredUpgrades)
            {
                if (KnownTech.Contains(uppy.Key.TechTypes.forSeamoth)
                    || KnownTech.Contains(uppy.Key.TechTypes.forExosuit)
                    || KnownTech.Contains(uppy.Key.TechTypes.forCyclops)
                    || KnownTech.Contains(uppy.Key.TechTypes.forModVehicle))
                {
                    // don't trigger on already-known technologies
                    continue;
                }
                if (uppy.Key.UnlockAtStart || KnownTech.Contains(uppy.Key.UnlockWith))
                {
                    // These should be TechType.None if nothing was registered, but adding None to KnownTech is harmless, so we can just add them all.
                    KnownTech.Add(uppy.Value.forSeamoth, true);
                    KnownTech.Add(uppy.Value.forExosuit, true);
                    KnownTech.Add(uppy.Value.forCyclops, true);
                    KnownTech.Add(uppy.Value.forModVehicle, true);
                    unlockedTechs.Add(uppy.Key.TechTypes.forModVehicle.AsString());
                }
            }

            // report whether we had to forcibly unlock something, and what it was
            if (list.Count > 0 || unlockedTechs.Count > 0)
            {
                Logger.PDANote("Some Vehicle Framework technologies were unlocked.");
            }
            list.ForEach(x => Logger.Log($"Unlocked vehicle: {x.techType.AsString()}"));
            unlockedTechs.ForEach(x => Logger.Log($"Unlocked upgrade: {x}"));

            Component.DestroyImmediate(this);
        }
    }
}

using System;
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
            bool somethingHappened = false;
            yield return new WaitForSeconds(5f);

            //unlock vehicles that should be unlocked
            var list = Admin.VehicleManager.vehicleTypes
                .Where(v => v.mv.UnlockedWith == TechType.None || KnownTech.Contains(v.mv.UnlockedWith))
                .ToList();
            list.ForEach(v => KnownTech.Add(v.techType, true));

            //unlock upgrades that should be unlocked
            foreach(var uppy in Admin.UpgradeRegistrar.RegisteredUpgrades)
            {
                if(uppy.Key.UnlockAtStart || KnownTech.Contains(uppy.Key.UnlockWith))
                {
                    // These should be TechType.None if nothing was registered, but adding None to KnownTech is harmless, so we can just add them all.
                    KnownTech.Add(uppy.Value.forSeamoth, true);
                    KnownTech.Add(uppy.Value.forExosuit, true);
                    KnownTech.Add(uppy.Value.forCyclops, true);
                    KnownTech.Add(uppy.Value.forModVehicle, true);
                    somethingHappened = true;
                }
            }

            if(list.Count > 0 || somethingHappened)
            {
                Logger.PDANote("Some Vehicle Framework technologies were unlocked.");
            }

            Component.DestroyImmediate(this);
        }
    }
}

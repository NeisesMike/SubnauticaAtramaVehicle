﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VehicleFramework
{
    public class ModVehicleTether : MonoBehaviour
    {
        private ModVehicle currentMV = null;

        public void CatchTether(ModVehicle mv)
        {
            currentMV = mv;
            StartCoroutine(CheckTether());
        }

        public IEnumerator CheckTether()
        {
            while (true)
            {
                if (currentMV != null)
                {
                    bool shouldDropLeash = false;
                    foreach (var tethersrc in currentMV.TetherSources)
                    {
                        // TODO make this constant depend on the vehicle somehow
                        if (5f < Vector3.Distance(Player.main.transform.position, tethersrc.transform.position))
                        {
                            shouldDropLeash = true;
                            break;
                        }
                    }
                    if (shouldDropLeash)
                    {
                        currentMV.PlayerExit();
                        currentMV.GetComponent<TetherSource>().BreakTether();
                        yield break;
                    }
                }
                yield return new WaitForSeconds(0.25f);
            }
        }

    }
}

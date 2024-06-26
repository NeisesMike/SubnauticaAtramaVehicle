﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VehicleFramework
{
    public class HeadLightsController : MonoBehaviour, IPowerListener, IPlayerListener
	{
        private ModVehicle mv;
        private bool isHeadlightsOn = false;
        public bool isLive = true;
        public void Awake()
        {
            mv = GetComponent<ModVehicle>();
        }

        public virtual void Update()
        {
            if(mv as VehicleTypes.Submarine != null && !(mv as VehicleTypes.Submarine).IsPlayerPiloting())
            {
                return;
            }
            if (mv.IsPlayerDry && Player.main.GetRightHandDown() && !Player.main.GetPDA().isInUse)
            {
                ToggleHeadlights();
            }
        }
        public void EnableHeadlights()
        {
            if (isLive)
            {
                SetHeadLightsActive(true);
                if (VehicleManager.isWorldLoaded && mv.GetComponent<PingInstance>().enabled)
                {
                    mv.lightsOnSound.Stop();
                    mv.lightsOnSound.Play();
                }
                isHeadlightsOn = true;
            }
        }
        public void DisableHeadlights()
        {
            if (isLive)
            {
                SetHeadLightsActive(false);
                if (VehicleManager.isWorldLoaded && mv.GetComponent<PingInstance>().enabled)
                {
                    mv.lightsOffSound.Stop();
                    mv.lightsOffSound.Play();
                }
                isHeadlightsOn = false;
            }
        }
        public void ToggleHeadlights()
        {
            if (mv.IsPowered())
            {
                if (isHeadlightsOn)
                {
                    DisableHeadlights();
                }
                else
                {
                    EnableHeadlights();
                }
            }
            else
            {
                isHeadlightsOn = false;
            }
        }
        public void SetVolumetricLightsActive(bool enabled)
        {
            if (isLive)
            {
                foreach (GameObject light in mv.volumetricLights)
                {
                    bool result = enabled;
                    result &= mv.IsPowered();
                    if (mv as VehicleTypes.Submarine != null)
                    {
                        result &= !(mv as VehicleTypes.Submarine).IsPlayerInside();
                    }
                    result &= !mv.IsPlayerDry;
                    light.SetActive(result);
                }
            }
        }
        public void SetHeadLightsActive(bool enabled)
        {
            if (isLive)
            {
                foreach (GameObject light in mv.lights)
                {
                    light.SetActive(enabled && mv.IsPowered());
                }
                SetVolumetricLightsActive(enabled);
                if (enabled)
                {
                    mv.NotifyStatus(LightsStatus.OnHeadLightsOn);
                }
                else
                {
                    mv.NotifyStatus(LightsStatus.OnHeadLightsOff);
                }
            }
        }

        void IPowerListener.OnPowerUp()
        {
        }

        void IPowerListener.OnPowerDown()
        {
            DisableHeadlights();
        }

        void IPowerListener.OnBatterySafe()
        {
        }

        void IPowerListener.OnBatteryLow()
        {
        }

        void IPowerListener.OnBatteryNearlyEmpty()
        {
        }

        void IPowerListener.OnBatteryDepleted()
        {
            DisableHeadlights();
        }

        void IPlayerListener.OnPlayerEntry()
        {
            SetVolumetricLightsActive(false);
        }

        void IPlayerListener.OnPlayerExit()
        {
            SetVolumetricLightsActive(true);
        }

        void IPlayerListener.OnPilotBegin()
        {
            //EnableHeadlights();
        }

        void IPlayerListener.OnPilotEnd()
        {
        }

        void IPowerListener.OnBatteryDead()
        {
            DisableHeadlights();
        }

        void IPowerListener.OnBatteryRevive()
        {
        }
    }
}

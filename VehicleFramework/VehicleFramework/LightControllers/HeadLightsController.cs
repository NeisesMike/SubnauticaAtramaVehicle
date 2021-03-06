using System;
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

        public void Awake()
        {
            mv = GetComponent<ModVehicle>();
        }

        public virtual void Update()
        {
            if (mv.IsPlayerPiloting() && Player.main.GetRightHandDown() && !Player.main.GetPDA().isInUse)
            {
                ToggleHeadlights();
            }
        }
        public void EnableHeadlights()
        {
            if (!isHeadlightsOn)
            {
                SetHeadLightsActive(true);
                mv.lightsOnSound.Stop();
                mv.lightsOnSound.Play();
                isHeadlightsOn = !isHeadlightsOn;
            }
        }
        public void DisableHeadlights()
        {
            if (isHeadlightsOn)
            {
                SetHeadLightsActive(false);
                mv.lightsOffSound.Stop();
                mv.lightsOffSound.Play();
                isHeadlightsOn = !isHeadlightsOn;
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
            foreach (GameObject light in mv.volumetricLights)
            {
                light.SetActive(!mv.IsPlayerInside() && enabled && mv.IsPowered());
            }
        }
        public void SetHeadLightsActive(bool enabled)
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
            DisableHeadlights();
        }

        void IPowerListener.OnBatteryNearlyEmpty()
        {
            DisableHeadlights();
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
            EnableHeadlights();
        }

        void IPlayerListener.OnPilotEnd()
        {
            DisableHeadlights();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;

namespace R5Submersible
{
    public abstract class LightController : MonoBehaviour, IPowerListener
    {
        public abstract void DisableLight();
        public abstract void EnableLight();
        void IPowerListener.OnBatteryDead()
        {
            DisableLight();
        }

        void IPowerListener.OnBatteryDepleted()
        {
            EnableLight();
        }

        void IPowerListener.OnBatteryLow()
        {
            EnableLight();
        }

        void IPowerListener.OnBatteryNearlyEmpty()
        {
            EnableLight();
        }

        void IPowerListener.OnBatteryRevive()
        {
            EnableLight();
        }

        void IPowerListener.OnBatterySafe()
        {
            EnableLight();
        }

        void IPowerListener.OnPowerDown()
        {
            DisableLight();
        }

        void IPowerListener.OnPowerUp()
        {
            EnableLight();
        }
    }

    public class LampController : LightController
    {
        public Light light;
        public void Awake()
        {
            light = gameObject.GetComponent<Light>();
        }
        public override void DisableLight()
        {
            light.enabled = false;
        }
        public override void EnableLight()
        {
            light.enabled = true;
        }
    }

    public class EmissiveController : LightController
    {
        public Renderer light;
        public void Awake()
        {
            light = gameObject.GetComponent<Renderer>();
        }
        public override void DisableLight()
        {
            light.material.SetColor("_EmissionColor", Color.black);
        }
        public override void EnableLight()
        {
            light.material.SetColor("_EmissionColor", Color.white);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleFramework
{
    public enum PowerEvent
    {
        OnPowerUp,
        OnPowerDown,
        OnBatteryDead,
        OnBatteryRevive,
        OnBatterySafe,
        OnBatteryLow,
        OnBatteryNearlyEmpty,
        OnBatteryDepleted
    }
    public interface IPowerListener
    {
        void OnPowerUp();
        void OnPowerDown();
        void OnBatteryDead();
        void OnBatteryRevive();
        void OnBatterySafe();
        void OnBatteryLow();
        void OnBatteryNearlyEmpty();
        void OnBatteryDepleted();
    }
}

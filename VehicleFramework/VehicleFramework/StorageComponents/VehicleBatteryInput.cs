﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleFramework
{
    public class VehicleBatteryInput : HandTarget, IHandTarget
	{
		public EnergyMixin mixin;
		public string tooltip;

		public void OnHandHover(GUIHand hand)
		{
			HandReticle.main.SetInteractText(tooltip);
			HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
		}

		public void OnHandClick(GUIHand hand)
		{
			mixin.InitiateReload();
		}
	}
}

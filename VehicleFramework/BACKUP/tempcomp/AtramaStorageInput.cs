﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AtramaVehicle
{
	public class AtramaStorageInput : HandTarget, IHandTarget
	{
		public override void Awake()
		{
			base.Awake();
			this.tr = base.GetComponent<Transform>();
			this.UpdateColliderState();

			atrama = transform.parent.gameObject.GetComponent<AtramaVehicle>();
		}

		private void Update()
		{

		}

		private void OnDisable()
		{

		}

		private void ChangeFlapState(bool open, bool pda = false)
		{
			Utils.PlayFMODAsset(open ? this.openSound : this.closeSound, base.transform, 1f);
			OpenPDA();
		}

		private void OpenPDA()
		{
			ItemsContainer storageInSlot = gameObject.GetComponent<AtramaStorageContainer>().container;
			if (storageInSlot != null)
			{
				PDA pda = Player.main.GetPDA();
				Inventory.main.SetUsedStorage(storageInSlot, false);
				if (!pda.Open(PDATab.Inventory, this.tr, new PDA.OnClose(this.OnClosePDA), -1f))
				{
					this.OnClosePDA(pda);
					return;
				}
			}
			else
			{
				this.OnClosePDA(null);
			}
		}

		private void OnClosePDA(PDA pda)
		{
			Utils.PlayFMODAsset(this.closeSound, base.transform, 1f);
		}

		private void UpdateColliderState()
		{
			if (this.collider != null)
			{
				this.collider.enabled = (this.state && this.dockType != Vehicle.DockType.Cyclops);
			}
		}

		public void SetEnabled(bool state)
		{
			if (this.state == state)
			{
				return;
			}
			this.state = state;
			this.UpdateColliderState();
			if (this.model != null)
			{
				this.model.SetActive(state);
			}
		}

		public void OpenFromExternal()
		{
			ItemsContainer storageInSlot = this.atrama.GetStorageInSlot(this.slotID, TechType.VehicleStorageModule);
			if (storageInSlot != null)
			{
				PDA pda = Player.main.GetPDA();
				Inventory.main.SetUsedStorage(storageInSlot, false);
				pda.Open(PDATab.Inventory, null, null, -1f);
			}
		}

		public void SetDocked(Vehicle.DockType dockType)
		{
			this.dockType = dockType;
			this.UpdateColliderState();
		}

		public void OnHandHover(GUIHand hand)
		{
			HandReticle.main.SetInteractText("AtramaStorageOpen");
			HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
		}


		public void OnHandClick(GUIHand hand)
		{
			this.ChangeFlapState(true, true);
		}

		public AtramaVehicle atrama;

		public int slotID = -1;

		public GameObject model;

		public Collider collider;

		public float timeOpen = 0.5f;

		public float timeClose = 0.25f;

		public FMODAsset openSound;

		public FMODAsset closeSound;

		private Transform tr;

		private Vehicle.DockType dockType;

		private bool state;
	}

}

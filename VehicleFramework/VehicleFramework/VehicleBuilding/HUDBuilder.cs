﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VehicleFramework
{
    /*
     * This class controls building and configuring the mod vehicle HUD
     * We have to work differently for VR
     */
    public static class HUDBuilder
    {
        public static bool IsVR = false;
        static List<GameObject> GetAllObjectsInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                objectsInScene.Add(go);
            }

            return objectsInScene;
        }
        public static GameObject TryGetVRVehicleCanvas()
        {
            // Here we ensure we have a reference to the freshest copy of VRVehicleCanvas
            // The base game has a memory leak, in which VRVehicleCanvas is not cleaned up across exit-reload
            // so we fix that here
            GameObject VRVehicleCanvas = null;
            foreach (GameObject go in GetAllObjectsInScene())
            {
                if (go.name == "VRVehicleCanvas")
                {
                    if (go.transform.Find("ModVehicle") is null)
                    {
                        VRVehicleCanvas = go;
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(go);
                    }
                }
            }
            if (VRVehicleCanvas)
            {
                GameObject seenTag = GameObject.Instantiate(new GameObject(), VRVehicleCanvas.transform);
                seenTag.name = "ModVehicle";
            }
            return VRVehicleCanvas;
        }
        public static void DecideBuildHUD()
        {
            GameObject VRVehicleCanvas = TryGetVRVehicleCanvas();
            if(VRVehicleCanvas)
            {
                IsVR = true;
                BuildVRHUD(VRVehicleCanvas);
            }
            else
            {
                BuildNormalHUD();
            }
        }
        public static void BuildNormalHUD()
        {
            // copy the seamoth hud for now
            GameObject seamothHUDElementsRoot= uGUI.main.transform.Find("ScreenCanvas/HUD/Content/Seamoth").gameObject;
            GameObject mvHUDElementsRoot = GameObject.Instantiate(seamothHUDElementsRoot, uGUI.main.transform.Find("ScreenCanvas/HUD/Content"));
            mvHUDElementsRoot.name = "ModVehicle";

            uGUI_VehicleHUD ret = uGUI.main.transform.Find("ScreenCanvas/HUD").gameObject.EnsureComponent<uGUI_VehicleHUD>();
            ret.root = mvHUDElementsRoot;
            ret.textHealth = mvHUDElementsRoot.transform.Find("Health").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textPower = mvHUDElementsRoot.transform.Find("Power").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperature = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperatureSuffix = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue/TemperatureSuffix").GetComponent<TMPro.TextMeshProUGUI>();

            // copy the CameraScannerRoom hud for now
            GameObject cameraScannerRoomObj = uGUI.main.transform.Find("ScreenCanvas/HUD/Content/CameraScannerRoom").gameObject;
            GameObject droneHUDElementsRoot = GameObject.Instantiate(cameraScannerRoomObj, mvHUDElementsRoot.transform);
            droneHUDElementsRoot.name = "VFDrone";

            GameObject.Destroy(droneHUDElementsRoot.transform.Find("HealthBackground").gameObject);
            GameObject.Destroy(droneHUDElementsRoot.transform.Find("PingCanvas").gameObject);
            GameObject.Destroy(droneHUDElementsRoot.transform.Find("PowerBackground").gameObject);
            droneHUDElementsRoot.transform.localPosition = new Vector3(-730.410f, 334.763f, 0f);
            ret.droneHUD = droneHUDElementsRoot;

        }
        public static void BuildVRHUD(GameObject VRVehicleCanvas)
        {
            // Now we want to add our ModVehicle HUD to the standard HUD.
            // We don't want to use the actual "vr vehicle hud"
            // but we're going to grab the copy of the seamoth HUD from the "vr vehicle hud"
            GameObject seamothHUDElementsRoot = VRVehicleCanvas.transform.Find("Seamoth").gameObject;
            GameObject mvHUDElementsRoot = GameObject.Instantiate(seamothHUDElementsRoot, uGUI.main.transform.Find("ScreenCanvas/HUD/Content"));
            mvHUDElementsRoot.name = "ModVehicle";
            mvHUDElementsRoot.transform.localPosition = new Vector3(245.2f, -163.5f, 0);
            mvHUDElementsRoot.transform.localScale = 0.8f * Vector3.one;

            // Finally we need to add and configure a controller for our new HUD object
            uGUI_VehicleHUD ret = uGUI.main.transform.Find("ScreenCanvas/HUD").gameObject.EnsureComponent<uGUI_VehicleHUD>();
            ret.root = mvHUDElementsRoot;
            ret.textHealth = mvHUDElementsRoot.transform.Find("Health").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textPower = mvHUDElementsRoot.transform.Find("Power").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperature = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperatureSuffix = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue/TemperatureSuffix").GetComponent<TMPro.TextMeshProUGUI>();
        }
        public static void BuildVRHUD_OLD()
        {

            List<GameObject> GetAllObjectsInScene()
            {
                List<GameObject> objectsInScene = new List<GameObject>();

                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                        continue;

                    objectsInScene.Add(go);
                }

                return objectsInScene;
            }


            GameObject VRVehicleCanvas = null;


            foreach (GameObject go in GetAllObjectsInScene())
            {
                if(go.name == "VRVehicleCanvas")
                {
                    if(go.transform.Find("ModVehicle") is null)
                    {
                        VRVehicleCanvas = go;
                    }
                }
            }

            GameObject seamothHUDElementsRoot = VRVehicleCanvas.transform.Find("Seamoth").gameObject;
            GameObject mvHUDElementsRoot = GameObject.Instantiate(seamothHUDElementsRoot, VRVehicleCanvas.transform);
            mvHUDElementsRoot.name = "ModVehicle";

            uGUI_VehicleHUD ret = uGUI.main.transform.Find("ScreenCanvas/HUD").gameObject.EnsureComponent<uGUI_VehicleHUD>();
            ret.root = mvHUDElementsRoot;
            ret.textHealth = mvHUDElementsRoot.transform.Find("Health").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textPower = mvHUDElementsRoot.transform.Find("Power").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperature = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue").GetComponent<TMPro.TextMeshProUGUI>();
            ret.textTemperatureSuffix = mvHUDElementsRoot.transform.Find("Temperature/TemperatureValue/TemperatureSuffix").GetComponent<TMPro.TextMeshProUGUI>();
        }
    }
}

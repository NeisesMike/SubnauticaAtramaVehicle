using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.VehicleParts;
using VehicleFramework.VehicleTypes;
using UnityEngine.U2D;

namespace R5Submersible
{
    public class R5 : Submersible
    {
        public static GameObject model = null;
        public static Atlas.Sprite pingSprite = null;
        public static Atlas.Sprite crafterSprite = null;

        public static void GetAssets()
        {
            // load the asset bundle
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "assets/renault-submersible"));
            if (myLoadedAssetBundle == null)
            {
                Logger.Log("Failed to load AssetBundle!");
                return;
            }

            System.Object[] arr = myLoadedAssetBundle.LoadAllAssets();
            foreach (System.Object obj in arr)
            {
                if (obj.ToString().Contains("SpriteAtlas"))
                {
                    SpriteAtlas thisAtlas = (SpriteAtlas)obj;

                    //Sprite ping = thisAtlas.GetSprite("PingSprite");
                    Sprite ping = thisAtlas.GetSprite("profile");
                    pingSprite = new Atlas.Sprite(ping);

                    Sprite ping3 = thisAtlas.GetSprite("CrafterSprite");
                    crafterSprite = new Atlas.Sprite(ping3);
                }
                else if (obj.ToString().Contains("R5"))
                        {
                    model = (GameObject)obj;
                }
                else
                {
                    Logger.Log(obj.ToString());
                }
            }
        }

        public override Dictionary<TechType, int> Recipe
        {
            get
            {
                Dictionary<TechType, int> recipe = new Dictionary<TechType, int>();
                recipe.Add(TechType.PlasteelIngot, 1);
                recipe.Add(TechType.Lubricant, 1);
                recipe.Add(TechType.EnameledGlass, 2);
                recipe.Add(TechType.Lead, 1);
                recipe.Add(TechType.AdvancedWiringKit, 1);
                recipe.Add(TechType.ComputerChip, 2);
                recipe.Add(TechType.PowerCell, 1);
                return recipe;
            }
        }

        public static IEnumerator Register()
        {
            Submersible r5 = model.EnsureComponent<R5>() as Submersible;
            yield return UWE.CoroutineHost.StartCoroutine(VehicleRegistrar.RegisterVehicle(r5));
        }

        public override string vehicleDefaultName
        {
            get
            {
                Language main = Language.main;
                if (!(main != null))
                {
                    return "R5";
                }
                return main.Get("R5DefaultName");
            }
        }

        public override string Description
        {
            get
            {
                return "The Renault R5 imagined as a submersible.";
            }
        }

        public override string EncyclopediaEntry
        {
            get
            {
                /*
                 * The Formula:
                 * 2 or 3 sentence blurb
                 * Features
                 * Advice
                 * Ratings
                 * Kek
                 */
                string ency = "The R5 Submersible is a new take on an old classic.";
                ency += "\nIt features:\n";
                ency += "- Rapid acceleration in all directions, but only a high top speed moving forward. \n";
                //ency += "- One power cell in each of the two small thrusters. \n";
                ency += "\nRatings:\n";
                ency += "- Top Speed: 12m/s \n";
                ency += "- Acceleration: 6m/s/s \n";
                ency += "- Distance per Power Cell: 7km \n";
                ency += "- Crush Depth: 250 \n";
                ency += "- Max Crush Depth (upgrade required): 1100 \n";
                ency += "- Upgrade Slots: 8 \n";
                //ency += "- Dimensions: 3.5m x 3.5m x 3.1m \n";
                ency += "- Persons: 1\n";
                return ency;
            }
        }

        public override GameObject VehicleModel
        {
            get
            {
                return model;
            }
        }

        public override GameObject StorageRootObject
        {
            get
            {
                return transform.Find("StorageRoot").gameObject;
            }
        }

        public override GameObject ModulesRootObject
        {
            get
            {
                return transform.Find("ModulesRoot").gameObject;
            }
        }

        public override VehiclePilotSeat PilotSeat
        {
            get
            {
                VehicleFramework.VehicleParts.VehiclePilotSeat vps = new VehicleFramework.VehicleParts.VehiclePilotSeat();
                Transform mainSeat = transform.Find("PilotSeat");
                vps.Seat = mainSeat.gameObject;
                vps.SitLocation = mainSeat.Find("SitLocation").gameObject;
                vps.LeftHandLocation = mainSeat;
                vps.RightHandLocation = mainSeat;
                return vps;
            }
        }

        public override List<VehicleHatchStruct> Hatches
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleHatchStruct>();

                VehicleFramework.VehicleParts.VehicleHatchStruct interior_vhs = new VehicleFramework.VehicleParts.VehicleHatchStruct();
                Transform intHatch = transform.Find("Hatches/driver");
                interior_vhs.Hatch = intHatch.gameObject;
                interior_vhs.ExitLocation = intHatch.Find("ExitLocation");
                interior_vhs.SurfaceExitLocation = intHatch.Find("ExitLocation");
                list.Add(interior_vhs);
                return list;
            }
        }

        public override List<VehicleStorage> InnateStorages
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleStorage>();
                VehicleFramework.VehicleParts.VehicleStorage thisVS = new VehicleFramework.VehicleParts.VehicleStorage();
                Transform thisStorage = transform.Find("InnateStorages/Cube");
                thisVS.Container = thisStorage.gameObject;
                thisVS.Height = 8;
                thisVS.Width = 6;
                list.Add(thisVS);

                return list;
            }
        }

        public override List<VehicleStorage> ModularStorages
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleStorage>();
                return list;
            }
        }

        public override List<VehicleUpgrades> Upgrades
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleUpgrades>();
                VehicleFramework.VehicleParts.VehicleUpgrades vu = new VehicleFramework.VehicleParts.VehicleUpgrades();
                vu.Interface = transform.Find("UpgradesInterface/Cube").gameObject;
                vu.Flap = vu.Interface;
                list.Add(vu);
                return list;
            }
        }

        public override List<VehicleBattery> Batteries
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleBattery>();

                VehicleFramework.VehicleParts.VehicleBattery vb1 = new VehicleFramework.VehicleParts.VehicleBattery();
                vb1.BatterySlot = transform.Find("Batteries/driver").gameObject;
                list.Add(vb1);

                VehicleFramework.VehicleParts.VehicleBattery vb2 = new VehicleFramework.VehicleParts.VehicleBattery();
                vb2.BatterySlot = transform.Find("Batteries/passenger").gameObject;
                list.Add(vb2);

                return list;
            }
        }

        public override List<VehicleBattery> BackupBatteries
        {
            get
            {
                return null;
            }
        }

        public override List<VehicleFloodLight> HeadLights
        {
            get
            {
                var list = new List<VehicleFramework.VehicleParts.VehicleFloodLight>();

                VehicleFramework.VehicleParts.VehicleFloodLight left = new VehicleFramework.VehicleParts.VehicleFloodLight
                {
                    Light = transform.Find("lights_parent/leftlight").gameObject,
                    Angle = 70,
                    Color = Color.white,
                    Intensity = 1.2f,
                    Range = 70f
                };
                list.Add(left);

                VehicleFramework.VehicleParts.VehicleFloodLight right = new VehicleFramework.VehicleParts.VehicleFloodLight
                {
                    Light = transform.Find("lights_parent/rightlight").gameObject,
                    Angle = 70,
                    Color = Color.white,
                    Intensity = 1.3f,
                    Range = 70f
                };
                list.Add(right);

                return list;
            }
        }

        public override List<GameObject> WaterClipProxies
        {
            get
            {
                var list = new List<GameObject>();
                foreach (Transform child in transform.Find("WaterClipProxies"))
                {
                    list.Add(child.gameObject);
                }
                return list;
            }
        }

        public override List<GameObject> CanopyWindows
        {
            get
            {
                var list = new List<GameObject>();
                list.Add(transform.Find("geo/Body/door_L/polySurface387").gameObject);
                list.Add(transform.Find("geo/Body/door_R/polySurface386").gameObject);
                list.Add(transform.Find("geo/Body/geomidPolypCube8font_front_side_glasas").gameObject);
                list.Add(transform.Find("geo/li_glass/textureEditorIsolateSelectSetgeolipolySurface410polySurface136li_glasspolySurface210polySurface207").gameObject);
                list.Add(transform.Find("geo/li_glass/polySurface212polySurface208textureEditorIsolateSelectSetgeolipolySurface410polySurface136li_glass").gameObject);
                //list.Add(transform.Find("geo/li_glass/polySurface399").gameObject); // rear lights

                return list;
            }
        }

        public override GameObject BoundingBox
        {
            get
            {
                return transform.Find("BoundingBox").gameObject;
            }
        }

        public override GameObject CollisionModel
        {
            get
            {
                return transform.Find("CollisionModel").gameObject;
            }
        }

        public override VehicleFramework.Engines.ModVehicleEngine Engine
        {
            get
            {
                return gameObject.EnsureComponent<VehicleFramework.Engines.CricketEngine>();
            }
        }

        public override Atlas.Sprite PingSprite
        {
            get
            {
                return pingSprite;
            }
        }

        public override int BaseCrushDepth
        {
            get
            {
                return 250;
                // Degasi 1 @ 250
                // seamoth at 200 now
            }
        }

        public override int CrushDepthUpgrade1
        {
            get
            {
                return 150; // 400
                // seamoth at 300 now
            }
        }

        public override int CrushDepthUpgrade2
        {
            get
            {
                return 250; // 650
                // Degasi 2 @ 500
                // seamoth at 500 now
            }
        }

        public override int CrushDepthUpgrade3
        {
            get
            {
                return 450; // 1100, Lost River
                // 1700 end game
                // seamoth at 900 now
            }
        }

        public override int MaxHealth
        {
            get
            {
                return 650;
            }
        }

        public override int Mass
        {
            get
            {
                return 2500;
            }
        }

        public override int NumModules
        {
            get
            {
                return 8;
            }
        }

        public override bool HasArms
        {
            get
            {
                return false;
            }
        }
        public override Atlas.Sprite CraftingSprite
        {
            get
            {
                return crafterSprite;
            }
        }
        public static readonly Color lemonChrome = new Color(255f / 255f, 205f / 255f, 0f / 255f, 1f);
        public override Color ConstructionGhostColor { get; set; } = lemonChrome;
        public override Color ConstructionWireframeColor { get; set; } = lemonChrome;
        public override float TimeToConstruct { get; set; } = 12f;
        public Light interiorLamp;
        public override void Awake()
        {
            base.Awake();
            SetupSteering();
            SetupLamp();
            SetupEmissives();
            SetupJets();
        }
        public override void Start()
        {
            base.Start();
            voice.voice = VehicleFramework.VoiceManager.GetVoice(PluginInfo.FRENCH);
            voice.blockVoiceChange = false; 
        }
        public override void PlayerEntry()
        {
            base.PlayerEntry();
            interiorLamp.intensity = 1f;
        }
        public override void PlayerExit()
        {
            base.PlayerExit();
            interiorLamp.intensity = 2.2f;
        }

        private void SetupSteering()
        {
            SteeringWheel steeringWheel = transform.Find("geo/stearing/geostearing_DisplaymidPolypolySurface110").gameObject.AddComponent<SteeringWheel>();
            steeringWheel.useRigidbody = useRigidbody;
        }

        private void SetupLamp()
        {
            GameObject lamp = transform.Find("InteriorLight").gameObject;
            interiorLamp = lamp.GetComponent<Light>();
            lamp.AddComponent<LampController>();
        }

        private void SetupEmissives()
        {
            transform.Find("geo/li_glass/light_glassgeolimidPolypolySurface85").gameObject.AddComponent<EmissiveController>();
            transform.Find("geo/Body/geobodyvrayLiMeshProperties1midPolypolySurface104_light").gameObject.AddComponent<EmissiveController>();
            transform.Find("geo/Body/geobodymidPolypolySurface97polySurface261_light").gameObject.AddComponent<EmissiveController>();
            transform.Find("geo/li_glass/polySurface400_light").gameObject.AddComponent<EmissiveController>();
            transform.Find("geo/li_glass/polySurface401_light").gameObject.AddComponent<EmissiveController>();
            transform.Find("geo/li_glass/polySurface402_light").gameObject.AddComponent<EmissiveController>();
        }
        private void SetupJets()
        {
            transform.Find("geo/Body/cilender_F_L").gameObject.AddComponent<JetController>().Initialize(-0.03816858f, -0.04636f);
            transform.Find("geo/Body/cilender_F_R").gameObject.AddComponent<JetController>().Initialize(0.03747902f, 0.04568f);
            transform.Find("geo/Body/cilender_B_L").gameObject.AddComponent<JetController>().Initialize(-0.03473215f, -0.04309f);
            transform.Find("geo/Body/cilender_B_R").gameObject.AddComponent<JetController>().Initialize(0.03457356f, 0.04321f);
        }
    }
}

using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VehicleFramework.Admin;
using System.Linq;

// PURPOSE: allow custom save file sprites to be displayed
// VALUE: High.

namespace VehicleFramework.Patches
{
    // See also: MainMenuLoadPanelPatcher
    [HarmonyPatch(typeof(SaveLoadManager))]
    public class SaveLoadManagerPatcher
    {
        public const string SaveFileSpritesFileName = "SaveFileSprites";
        internal static Dictionary<string, List<string>> hasTechTypeGameInfo = new();

        internal static void SerializeHasVehicleTechTypes()
        {
            var techTypeData = VehicleManager.vehicleTypes.Select(x => x.techType).Where(x => GameInfoIcon.Has(x)).Select(x => x.AsString()).ToList();
            VehicleFramework.SaveLoad.JsonInterface.Write(SaveFileSpritesFileName, techTypeData);
        }

        private static string GetSpritesSavePath(string slotName)
        {
            string savePath = "dummy";
            string subnauticaPath;
            try
            {
                subnauticaPath = Directory.GetParent(BepInEx.Paths.BepInExRootPath).FullName;
            }
            catch (System.Exception e)
            {
                Logger.DebugException("SaveLoadManagerPatcher.GetSpritesSavePath failed to get parent directory.", e);
                return savePath;
            }
            try
            {
                savePath = Path.Combine(subnauticaPath, "SNAppData", "SavedGames", slotName, SaveLoad.JsonInterface.SaveFolderName, $"{SaveFileSpritesFileName}.json");
            }
            catch (System.Exception e)
            {
                Logger.DebugException("SaveLoadManagerPatcher.GetSpritesSavePath failed to get parent directory.", e);
            }
            return savePath;
        }

        // This patch collects hasTechTypeGameInfo, in order to have save file sprites displayed on the save cards
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SaveLoadManager.RegisterSaveGame))]
        public static void SaveLoadManagerRegisterSaveGamePostfix(string slotName)
        {
            string savePath = GetSpritesSavePath(slotName);
            if (!File.Exists(savePath))
            {
                Logger.DebugLog("SaveLoadManager.RegisterSaveGamePostfix failed to find the save game json file!");
                return;
            }
            try
            {
                string jsonContent = File.ReadAllText(savePath);
                List<string>? hasTechTypes = JsonConvert.DeserializeObject<List<string>>(jsonContent);
                if (hasTechTypes != null)
                {
                    if (hasTechTypeGameInfo.ContainsKey(slotName))
                    {
                        hasTechTypeGameInfo[slotName] = hasTechTypes;
                    }
                    else
                    {
                        hasTechTypeGameInfo.Add(slotName, hasTechTypes);
                    }
                }
            }
            catch (System.Exception e)
            {
                Logger.LogException("SaveLoadManager.RegisterSaveGamePostfix: Could not read json file!", e);
            }
        }
    }
}

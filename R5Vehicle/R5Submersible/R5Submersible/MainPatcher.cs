using HarmonyLib;
using BepInEx;
using System.IO;
using System.Reflection;
using Nautilus.Handlers;

namespace R5Submersible
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(VehicleFramework.PluginInfo.PLUGIN_GUID)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    public class MainPatcher : BaseUnityPlugin
    {
        internal static R5Config r5config { get; private set; }
        public void Awake()
        {
            R5Submersible.Logger.MyLog = base.Logger;
            R5.GetAssets();
            r5config = OptionsPanelHandler.RegisterModOptions<R5Config>();
        }
        public void Start()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            UWE.CoroutineHost.StartCoroutine(VehicleFramework.VoiceManager.RegisterVoice(PluginInfo.ENGLISH, modFolder));
            UWE.CoroutineHost.StartCoroutine(VehicleFramework.VoiceManager.RegisterVoice(PluginInfo.FRENCH, modFolder));
            UWE.CoroutineHost.StartCoroutine(R5.Register());

        }
    }
}

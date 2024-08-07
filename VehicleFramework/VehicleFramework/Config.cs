﻿using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace VehicleFramework
{
    [Menu("Vehicle Framework Options")]
    public class VehicleFrameworkConfig : ConfigFile
    {
        [Toggle("Flashing Lights")]
        public bool isFlashingLightsEnabled = false;

        [Slider("AI Voice Volume", Step = 1f, DefaultValue = 100, Min = 0, Max = 100)]
        public float aiVoiceVolume = 50f;

        [Toggle("Enable Debug Logs")]
        public bool isDebugLogging = false;

        [Choice("Autopilot Voice", Options = new[] { "ShirubaFoxy", "Chels-E", "Mikjaw", "Turtle", "Salli" }), OnChange(nameof(GrabNewVoiceLines))]
        public string voiceChoice = "ShirubaFoxy";
        public void GrabNewVoiceLines()
        {
            if (Player.main != null)
            {
                foreach (var tmp in VoiceManager.voices)
                {
                    tmp.SetVoice(VoiceManager.GetVoice(voiceChoice));
                }
            }
        }

        [Choice("Engine Sounds", Options = new[] { "ShirubaFoxy" }), OnChange(nameof(GrabNewEngineSounds))]
        public string engineSounds = "ShirubaFoxy";
        public void GrabNewEngineSounds()
        {
            if (Player.main != null)
            {
                foreach (var tmp in EngineSoundsManager.engines)
                {
                    tmp.SetVoice(EngineSoundsManager.GetVoice(engineSounds));
                }
            }
        }

        [Slider("Engine Volume", Step = 1f, DefaultValue = 100, Min = 0, Max = 100)]
        public float engineVolume = 50f;

        [Keybind("Next Camera")]
        public KeyCode nextCamera = KeyCode.F;

        [Keybind("Previous Camera")]
        public KeyCode previousCamera = KeyCode.T;

        [Keybind("Exit Camera")]
        public KeyCode exitCamera = KeyCode.V;

        [Toggle("Fragment Experience", Tooltip = "Enable scannable fragments. Leave unchecked if adding this mod to an existing world. Requires Subnautica reboot when changed.")]
        public bool isFragmentExperience = true;

    }
}

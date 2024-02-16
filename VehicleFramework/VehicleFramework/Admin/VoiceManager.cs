﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using System.IO;

namespace VehicleFramework
{
    public enum KnownVoices
    {
        // Only add new items to the end of this list
        // That way, dependent mods won't get bamboozled
        ShirubaFoxy,
        Airon,
        Chelse,
        Mikjaw,
        Turtle,
        Salli
    }
    public class VehicleVoice
    {
        public AudioClip BatteriesDepleted;
        public AudioClip BatteriesNearlyEmpty;
        public AudioClip PowerLow;
        public AudioClip EnginePoweringDown;
        public AudioClip EnginePoweringUp;
        public AudioClip Goodbye;
        public AudioClip HullFailureImminent;
        public AudioClip HullIntegrityCritical;
        public AudioClip HullIntegrityLow;
        public AudioClip Leveling;
        public AudioClip WelcomeAboard;
        public AudioClip OxygenProductionOffline;
        public AudioClip WelcomeAboardAllSystemsOnline;
        public AudioClip MaximumDepthReached;
        public AudioClip PassingSafeDepth;
        public AudioClip LeviathanDetected;
        public AudioClip UhOh;
    }
    public static class VoiceManager
    {
        public static List<AutoPilotVoice> voices = new List<AutoPilotVoice>();

        // voice names : voices
        private static Dictionary<string, VehicleVoice> vehicleVoices = new Dictionary<string, VehicleVoice>();
        // vehicle names : voice names
        private static Dictionary<string, string> defaultVoices = new Dictionary<string, string>();
        public static AudioClip silence;
        public static VehicleVoice silentVoice = new VehicleVoice();
        public static void RegisterVoice(string name, VehicleVoice voice)
        {
            try
            {
                vehicleVoices.Add(name, voice);
            }
            catch (ArgumentException e)
            {
                Logger.Warn("Tried to register a voice using a name that already exists: " + name + ". " + e.Message);
                return;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to register a voice: " + e.Message);
                return;
            }
            Logger.Log("Successfully registered voice: " + name);
        }
        public static IEnumerator RegisterVoice(string name, bool isInCallingAssembly = false)
        {
            yield return LoadVoiceClips(name, vehicleVoice =>
            {
                // Once the voice is loaded, store it in the dictionary
                RegisterVoice(name, vehicleVoice);
            }, isInCallingAssembly);
        }
        public static VehicleVoice GetVoice(string name)
        {
            try
            {
                return vehicleVoices[name];
            }
            catch(KeyNotFoundException e)
            {
                Logger.Warn("That voice not found: " + name + ". " + e.Message);
            }
            catch(ArgumentNullException e)
            {
                Logger.Warn("That voice was null: " + e.Message);
            }
            catch(Exception e)
            {
                Logger.Error("GetVoice failed: " + e.Message);
            }
            return silentVoice;
        }
        public static void RegisterDefault(ModVehicle mv, string voice)
        {
            try
            {
                defaultVoices.Add(mv.name, voice);
            }
            catch (ArgumentException e)
            {
                Logger.Warn("Tried to register a default voice for a vehicle that already had a default voice." + e.Message);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to register a default voice: " + e.Message);
            }
        }
        public static void RegisterDefault(ModVehicle mv, KnownVoices voice)
        {
            RegisterDefault(mv, GetKnownVoice(voice));
        }
        public static VehicleVoice GetDefaultVoice(ModVehicle mv)
        {
            string defaultOption = "";
            try
            {
                defaultOption = defaultVoices[mv.name];
            }
            catch (KeyNotFoundException e)
            {
                Logger.Warn("Default voice option not found for vehicle: " + mv.name + ". " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Logger.Warn("That mv.name was null: " + e.Message);
            }
            catch (Exception e)
            {
                Logger.Error("GetDefaultVoice option failed: " + e.Message);
            }

            try
            {
                return vehicleVoices[defaultOption];
            }
            catch (KeyNotFoundException e)
            {
                Logger.Warn("Default voice not found for vehicle: " + mv.name + ". " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Logger.Warn("That default voice index was null: " + e.Message);
            }
            catch (Exception e)
            {
                Logger.Error("GetDefaultVoice failed: " + e.Message);
            }
            return GetVoice(MainPatcher.VFConfig.voiceChoice);
        }
        public static IEnumerator LoadAllVoices()
        {
            GetSilence();
            yield return RegisterVoice(GetKnownVoice(KnownVoices.ShirubaFoxy));
            yield return RegisterVoice(GetKnownVoice(KnownVoices.Chelse));
            yield return RegisterVoice(GetKnownVoice(KnownVoices.Airon));
            yield return RegisterVoice(GetKnownVoice(KnownVoices.Mikjaw));
            yield return RegisterVoice(GetKnownVoice(KnownVoices.Salli));
            yield return RegisterVoice(GetKnownVoice(KnownVoices.Turtle));
        }
        public static IEnumerator GetSilence()
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string autoPilotVoicesFolder = Path.Combine(modPath, "AutoPilotVoices");
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicesFolder + "/Silence.ogg", AudioType.OGGVORBIS);
            yield return www.SendWebRequest();
            if (www.isHttpError || www.isNetworkError)
            {
                Logger.Error("ERROR: Silence.ogg not found. Directory error.");
                yield break;
            }
            silence = DownloadHandlerAudioClip.GetContent(www);

            silentVoice = new VehicleVoice
            {
                BatteriesDepleted = silence,
                BatteriesNearlyEmpty = silence,
                PowerLow = silence,
                EnginePoweringDown = silence,
                EnginePoweringUp = silence,
                Goodbye = silence,
                HullFailureImminent = silence,
                HullIntegrityCritical = silence,
                HullIntegrityLow = silence,
                Leveling = silence,
                WelcomeAboard = silence,
                OxygenProductionOffline = silence,
                WelcomeAboardAllSystemsOnline = silence,
                MaximumDepthReached = silence,
                PassingSafeDepth = silence,
                LeviathanDetected = silence,
                UhOh = silence
            };

            yield break;
        }
        // Method signature with a callback to return the VehicleVoice instance
        public static IEnumerator LoadVoiceClips(string voice, Action<VehicleVoice> onComplete, bool isInCallingAssembly)
        {
            VehicleVoice returnVoice = new VehicleVoice();
            string modPath = "";
            if(isInCallingAssembly)
            {
                modPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }
            else
            {
                modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            string autoPilotVoicesFolder = Path.Combine(modPath, "AutoPilotVoices");
            string autoPilotVoicePath = Path.Combine(autoPilotVoicesFolder, voice) + "/";

            // List of clip names to load, corresponding to their fields in VehicleVoice
            string[] clipNames = {
            "BatteriesDepleted",
            "BatteriesNearlyEmpty",
            "PowerLow",
            "EnginePoweringDown",
            "EnginePoweringUp",
            "Goodbye",
            "HullFailureImminent",
            "HullIntegrityCritical",
            "HullIntegrityLow",
            "Leveling",
            "WelcomeAboard",
            "OxygenProductionOffline",
            "WelcomeAboardAllSystemsOnline",
            "MaximumDepthReached",
            "PassingSafeDepth",
            "LeviathanDetected",
            "UhOh"
        };

            foreach (string clipName in clipNames)
            {
                string path = "file://" + autoPilotVoicePath + clipName + ".ogg";
                yield return LoadAudioClip(path, clip =>
                {
                    // Use reflection to set the clip dynamically based on its name
                    clip.name = clipName;
                    typeof(VehicleVoice).GetField(clipName).SetValue(returnVoice, clip);
                },
                () =>
                {
                    // Handle error, potentially logging and assigning Silence
                    Logger.Warn($"WARNING: {clipName} could not be loaded. Assigning Silence.");
                    typeof(VehicleVoice).GetField(clipName).SetValue(returnVoice, silence);
                });
            }

            onComplete?.Invoke(returnVoice);
        }
        private static IEnumerator LoadAudioClip(string filePath, Action<AudioClip> onSuccess, Action onError)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.OGGVORBIS))
            {
                yield return www.SendWebRequest();

                if (www.isHttpError || www.isNetworkError)
                {
                    onError?.Invoke();
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        Logger.Error("Failed to retrieve AudioClip from file: " + filePath);
                    }
                    else
                    {
                        onSuccess?.Invoke(clip);
                    }
                }
            }
        }
        public static string GetKnownVoice(KnownVoices name)
        {
            switch (name)
            {
                case KnownVoices.ShirubaFoxy:
                    return "ShirubaFoxy";
                case KnownVoices.Airon:
                    return "Airon";
                case KnownVoices.Chelse:
                    return "Chels-E";
                case KnownVoices.Mikjaw:
                    return "Mikjaw";
                case KnownVoices.Turtle:
                    return "Turtle";
                case KnownVoices.Salli:
                    return "Salli";
                default:
                    break;
            }
            return "The KnownVoices enum is likely outdated";
        }
    }
}

using System;
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
    public class AutoPilotVoice : MonoBehaviour
    {
        public ModVehicle mv;
        public EnergyInterface aiEI;
        private List<AudioSource> speakers = new List<AudioSource>();
        private PriorityQueue<AudioClip> speechQueue = new PriorityQueue<AudioClip>();

        public AudioClip Silence;
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

        public void PauseSpeakers(bool pause)
        {
            foreach (var sp in speakers)
            {
                if(sp != null)
                {
                    if (pause)
                    {
                        sp.Pause();
                    }
                    else
                    {
                        sp.UnPause();
                    }
                }
            }
        }
        public void Awake()
        {
            mv = GetComponent<ModVehicle>();
            aiEI = mv.AIEnergyInterface;

            // register self with mainpatcher, for on-the-fly voice selection updating
            MainPatcher.voices.Add(this);
        }
        public void Start()
        {
            AudioSource speakerPtr;
            foreach (var ps in mv.PilotSeats)
            {
                speakerPtr = ps.Seat.AddComponent<AudioSource>();
                speakerPtr.spatialBlend = 1f;
                speakers.Add(speakerPtr);
            }
            foreach (var ps in mv.Hatches)
            {
                speakerPtr = ps.Hatch.AddComponent<AudioSource>();
                speakerPtr.spatialBlend = 1f;
                speakers.Add(speakerPtr);
            }
            foreach (var ps in mv.TetherSources)
            {
                speakerPtr = ps.AddComponent<AudioSource>();
                speakerPtr.spatialBlend = 1f;
                speakers.Add(speakerPtr);
            }
            foreach(var sp in speakers)
            {
                sp.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 1500;
            }

            //StartCoroutine(GetAudioClips());
            TryGetAllAudioClips(MainPatcher.Config.voiceChoice);
        }
        public void Update()
        {
            if (aiEI.hasCharge)
            {
                if (speechQueue.Count > 0)
                {
                    bool tmp = false;
                    foreach(var but in speakers)
                    {
                        if(but.isPlaying)
                        {
                            tmp = true;
                            break;
                        }
                    }
                    if (!tmp)
                    {
                        TryPlayNextClipInQueue();
                    }
                }
            }
            foreach (var speaker in speakers)
            {
                if (mv.IsPlayerInside())
                {
                    speaker.GetComponent<AudioLowPassFilter>().enabled = false;
                }
                else
                {
                    speaker.GetComponent<AudioLowPassFilter>().enabled = true;
                }
            }
            
        }
        public void TryPlayNextClipInQueue()
        {
            if (speechQueue.TryDequeue(out AudioClip clip))
            {
                var nearestSpeaker = speakers.First();
                float nearestSpeakerDist = Vector3.Distance(Player.main.transform.position, nearestSpeaker.transform.position);
                foreach(var speaker in speakers)
                {
                    var thisDist = Vector3.Distance(Player.main.transform.position, speaker.transform.position);
                    if (thisDist < nearestSpeakerDist)
                    {
                        nearestSpeaker = speaker;
                        nearestSpeakerDist = thisDist;
                    }
                }
                nearestSpeaker.volume = MainPatcher.Config.aiVoiceVolume / 100f;
                nearestSpeaker.clip = clip;
                nearestSpeaker.Play();
            }
        }
        public void EnqueueClipWithPriority(AudioClip clip, int priority)
        {
            if (mv && aiEI.hasCharge)
            {
                speechQueue.Enqueue(clip, priority);
            }
        }
        public void EnqueueClip(AudioClip clip)
        {
            if (mv && aiEI.hasCharge && clip)
            {
                speechQueue.Enqueue(clip, 0);
            }
        }
        public void TryGetAllAudioClips(string voiceChoice)
        {
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string autoPilotVoicesFolder = Path.Combine(modPath, "AutoPilotVoices");
            string autoPilotVoicePath = Path.Combine(autoPilotVoicesFolder, voiceChoice) + "/";

            IEnumerator GrabSilence()
            {
                UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicesFolder + "/Silence.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("ERROR: Silence.ogg not found. Directory error.");
                }
                else
                {
                    Silence = DownloadHandlerAudioClip.GetContent(www);
                }
                yield break;
            }
            // I loathe that there is no genericity here, but, for the life of me, I can't figure it out
            IEnumerator GrabAllClipsForCryingOutLoud()
            {
                UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "BatteriesDepleted.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: BatteriesDepleted.ogg");
                    BatteriesDepleted = Silence;
                }
                else
                {
                    BatteriesDepleted = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "BatteriesNearlyEmpty.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: BatteriesNearlyEmpty.ogg");
                    BatteriesNearlyEmpty = Silence;
                }
                else
                {
                    BatteriesNearlyEmpty = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "EnginePoweringDown.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: EnginePoweringDown.ogg");
                    EnginePoweringDown = Silence;
                }
                else
                {
                    EnginePoweringDown = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "EnginePoweringUp.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: EnginePoweringUp.ogg");
                    EnginePoweringUp = Silence;
                }
                else
                {
                    EnginePoweringUp = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "Goodbye.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: Goodbye.ogg");
                    Goodbye = Silence;
                }
                else
                {
                    Goodbye = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "HullFailureImminent.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: HullFailureImminent.ogg");
                    HullFailureImminent = Silence;
                }
                else
                {
                    HullFailureImminent = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "HullIntegrityCritical.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: HullIntegrityCritical.ogg");
                    HullIntegrityCritical = Silence;
                }
                else
                {
                    HullIntegrityCritical = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "HullIntegrityLow.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: HullIntegrityLow.ogg");
                    HullIntegrityLow = Silence;
                }
                else
                {
                    HullIntegrityLow = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "Leveling.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: Leveling.ogg");
                    Leveling = Silence;
                }
                else
                {
                    Leveling = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "LeviathanDetected.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: LeviathanDetected.ogg");
                    LeviathanDetected = Silence;
                }
                else
                {
                    LeviathanDetected = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "MaximumDepthReached.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: MaximumDepthReached.ogg");
                    MaximumDepthReached = Silence;
                }
                else
                {
                    MaximumDepthReached = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "OxygenProductionOffline.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: OxygenProductionOffline.ogg");
                    OxygenProductionOffline = Silence;
                }
                else
                {
                    OxygenProductionOffline = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "PassingSafeDepth.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: PassingSafeDepth.ogg");
                    PassingSafeDepth = Silence;
                }
                else
                {
                    PassingSafeDepth = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "PowerLow.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: PowerLow.ogg");
                    PowerLow = Silence;
                }
                else
                {
                    PowerLow = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "UhOh.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: UhOh.ogg");
                    UhOh = Silence;
                }
                else
                {
                    UhOh = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "WelcomeAboard.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: WelcomeAboard.ogg");
                    WelcomeAboard = Silence;
                }
                else
                {
                    WelcomeAboard = DownloadHandlerAudioClip.GetContent(www);
                }

                www = UnityWebRequestMultimedia.GetAudioClip("file://" + autoPilotVoicePath + "WelcomeAboardAllSystemsOnline.ogg", AudioType.OGGVORBIS);
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Logger.Log("WARNING: " + MainPatcher.Config.voiceChoice + " does not have the voice clip: WelcomeAboardAllSystemsOnline.ogg");
                    WelcomeAboardAllSystemsOnline = Silence;
                }
                else
                {
                    WelcomeAboardAllSystemsOnline = DownloadHandlerAudioClip.GetContent(www);
                }

                yield break;
            }

            StartCoroutine(GrabSilence());
            StartCoroutine(GrabAllClipsForCryingOutLoud());
        }
    }
}

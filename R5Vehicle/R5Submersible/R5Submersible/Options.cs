using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace R5Submersible
{
    [Menu("R5 Voice / Voix")]
    public class R5Config : ConfigFile
    {
        const string hovertext = "Utiliser la voix spéciale R5 / Use the special R5 voice";
        [Choice("Autopilot Voice/Voix", Tooltip = hovertext, Options = new[] {"English", "Francais"}), OnChange(nameof(GrabNewVoiceLines))]
        public string voiceChoice = "Francais";
        public void GrabNewVoiceLines()
        {
            if (Player.main != null)
            {
                foreach (var tmp in VehicleFramework.VoiceManager.voices.Where(x=>x.mv != null).Where(x => x.mv.name.ToLower().Contains("r5")))
                {
                    if(voiceChoice.Contains("English"))
                    {
                        tmp.SetVoice(VehicleFramework.VoiceManager.GetVoice(PluginInfo.ENGLISH));
                    }
                    else
                    {
                        tmp.SetVoice(VehicleFramework.VoiceManager.GetVoice(PluginInfo.FRENCH));
                    }
                }
            }
        }
    }
}

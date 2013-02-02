using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinectfinal
{
    class VoicePool
    {
        public Dictionary<VoiceControl.Turn, VoiceControl> voiceControlList = new Dictionary<VoiceControl.Turn, VoiceControl>();

        public VoicePool()
        {
            voiceControlList.Add( key: VoiceControl.Turn.turnPages, value: new TurnPages());
   
        }
    }
}

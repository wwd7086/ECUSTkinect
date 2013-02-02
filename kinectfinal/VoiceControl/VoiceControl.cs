using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace kinectfinal
{
    abstract class VoiceControl
    {
        //show function in use
        public enum Turn { none, turnPages};
        public static Turn turn { get; set; }

        //index to the mainwindow
        public static MainWindow main { get; set; }

        public abstract void PagesTurnViaVoice(KinectSensor _sensor);


    }
}
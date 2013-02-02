using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace kinectfinal
{
    class TurnPages : VoiceControl
    {
        private SpeechRecognitionEngine _sre;

        public override void PagesTurnViaVoice(KinectSensor _sensor)
        //private void PagesTurnViaVoice(KinectSensor _sensor)
        {
            // 等待4秒钟时间，让Kinect传感器初始化启动完成
            System.Threading.Thread.Sleep(4000);

            // 获取Kinect音频对象
            KinectAudioSource source = _sensor.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // 本示例中关闭“回声抑制模式”
            source.AutomaticGainControlEnabled = false; // 启用语音命令识别需要关闭“自动增益”

            RecognizerInfo ri = GetKinectRecognizer();

            if (ri == null)
            {
                MessageBox.Show("Could not find Kinect speech recognizer.");
                return;
            }

            _sre = new SpeechRecognitionEngine(ri.Id);

            // 添加上翻、下翻两个动作
            var directions = new Choices();
            directions.Add("up");
            directions.Add("down");

            var gb = new GrammarBuilder { Culture = ri.Culture };

            // 创建语法对象                                
            gb.Append(directions);

            //根据语言区域，创建语法识别对象
            var g = new Grammar(gb);

            // 将这些语法规则加载进语音识别引擎
            _sre.LoadGrammar(g);

            // 注册事件：有效语音命令识别、疑似识别、无效识别
            _sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            _sre.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(sre_SpeechHypothesized);
            _sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);

            // 初始化并启动 Kinect音频流
            Stream s = source.Start();
            _sre.SetInputToAudioStream(
                s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

            // 异步开启语音识别引擎，可识别多次
            _sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 语音命令识别处理，切换PPT展示页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //语音识别信心度超过70%
            if (e.Result.Confidence >= 0.7)
            {
                string direction = e.Result.Text.ToLower();
                if (direction == "up")
                {
                    System.Windows.Forms.SendKeys.SendWait("{Left}");                    
                }
                else if (direction == "down")
                {
                    System.Windows.Forms.SendKeys.SendWait("{Right}");                    
                }
            }
        }
    }
}

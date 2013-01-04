using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Kinect;

namespace kinectfinal
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //引入vb对键盘和鼠标的控制 实现ctrl+.....
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int flags, int dx, int dy, int data, UIntPtr extraInfo);

        [DllImport("user32.dll")]
        public static extern byte MapVirtualKey(byte wCode, int wMap);

        //helper of level
        public static bool compareValue(float a, float b, float dif)
        {
            if (Math.Abs(a - b) < dif)
                return true;
            else
                return false;
        }

        public static bool comparePoint(SkeletonPoint a, SkeletonPoint b, float dif)
        {
            if (compareValue(a.X, b.X, dif) && compareValue(a.Y, b.Y, dif) && compareValue(a.Z, b.Z, dif))
                return true;
            else
                return false;
        }
    }
}

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

namespace kinectfinal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 主窗口的配置
        KinectSensor _sensor;
        CoordinateMapper cosensor;
        //DispatcherTimer readyTimer;
        byte[] colorBytes;
        Skeleton[] skeletons;
        SolidColorBrush actBrush = new SolidColorBrush(Colors.Green);
        SolidColorBrush inactBrush = new SolidColorBrush(Colors.Red);
        // 各项控件和参数值的初始化命名和定义

        /////////declare
        ActionPool actionPool = new ActionPool();

        public MainWindow()
        {
            InitializeComponent();
            // 组件初始化
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            // 一旦收到数据就开始作处理
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /////////////init the levelaction
            LevelAction.main = this;

            _sensor = KinectSensor.KinectSensors.FirstOrDefault();
            // 定义数据源为默认的Kinect传感器或者是第一个连接的Kinect传感器
            if (_sensor == null)
            {
                MessageBox.Show("您没有正确连接Kinect传感器或正确安装相关驱动，请点击确定后断开Kinect检查驱动程序，并尝试重新连接Kinect。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                // 如果没有检测到传感器
                System.Environment.Exit(0);
            }
            cosensor = new CoordinateMapper(_sensor);
            _sensor.Start();
            // 启动传感器
            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            // 打开传感器色彩数据流
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            _sensor.SkeletonStream.Enable();
            _sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            _sensor.ElevationAngle = 0;

            Application.Current.Exit += new ExitEventHandler(Current_Exit);
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.AudioSource.Stop();
                _sensor.Stop();
                _sensor.Dispose();
                _sensor = null;
                // 关闭Kinect传感器，并标记退出
            }
        }


        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // Kinect彩色数据流
            using (var image = e.OpenColorImageFrame())
            {
                if (image == null)
                    return;

                if (colorBytes == null ||
                    colorBytes.Length != image.PixelDataLength)
                {
                    colorBytes = new byte[image.PixelDataLength];
                }
                // 初始化
                image.CopyPixelDataTo(colorBytes);

                int length = colorBytes.Length;
                for (int i = 0; i < length; i += 4)
                {
                    colorBytes[i + 3] = 255;
                }
                // 颜色的RGB信息的位处理初始化

                BitmapSource source = BitmapSource.Create(image.Width,
                    image.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    colorBytes,
                    image.Width * image.BytesPerPixel);
                videoImage.Source = source;
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //get the closestSkeleton
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                if (skeletons == null || skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);

                Skeleton closestSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked &&
                                                  s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                                            select s).OrderBy(s => s.Joints[JointType.Head].Position.Z)
                                                    .FirstOrDefault();

                if (closestSkeleton == null)
                    return;

                //list the joint needed by all the function
                LevelAction.rightHand = closestSkeleton.Joints[JointType.HandRight];
                LevelAction.leftHand = closestSkeleton.Joints[JointType.HandLeft];
                LevelAction.spine = closestSkeleton.Joints[JointType.Spine];

                //confirm the joint 
                if (LevelAction.rightHand.TrackingState != JointTrackingState.Tracked
                    || LevelAction.leftHand.TrackingState != JointTrackingState.Tracked
                    || LevelAction.spine.TrackingState != JointTrackingState.Tracked)
                    return;

                if (LevelAction.turn == LevelAction.Turn.none)
                {
                    //do confirm
                    foreach (KeyValuePair<LevelAction.Turn, LevelAction> actionElement in actionPool.levelActionList)
                        actionElement.Value.confirm();
                }
                else
                {
                    //do process
                    actionPool.levelActionList[LevelAction.turn].process();
                }

                //store the prevalue
                LevelAction.preLefthand = LevelAction.leftHand.Position;
                LevelAction.preRighthand = LevelAction.rightHand.Position;

                //show ellipse
                if (LevelAction.turn == LevelAction.Turn.none)
                {
                    SetEllipsePosition(ellipseLeftHand, LevelAction.leftHand, false);
                    SetEllipsePosition(ellipseRightHand, LevelAction.rightHand, false);
                    SetEllipsePosition(ellipseSpine, LevelAction.spine, false);
                }
                else
                {
                    bool[] pointInUse = actionPool.levelActionList[LevelAction.turn].pointInUse;
                    SetEllipsePosition(ellipseLeftHand, LevelAction.leftHand, pointInUse[0]);
                    SetEllipsePosition(ellipseRightHand, LevelAction.rightHand, pointInUse[1]);
                    SetEllipsePosition(ellipseSpine, LevelAction.spine, pointInUse[2]);
                }
                //show state
                state.Content = LevelAction.state;
                //show position 
                rightx.Content = LevelAction.rightHand.Position.X.ToString("0.00");
                righty.Content = LevelAction.rightHand.Position.Y.ToString("0.00");
                rightz.Content = LevelAction.rightHand.Position.Z.ToString("0.00");
                leftx.Content = LevelAction.leftHand.Position.X.ToString("0.00");
                lefty.Content = LevelAction.leftHand.Position.Y.ToString("0.00");
                leftz.Content = LevelAction.leftHand.Position.Z.ToString("0.00");

            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint, bool isHighlighted)
        {
            var point = cosensor.MapSkeletonPointToColorPoint(joint.Position, _sensor.ColorStream.Format);

            if (isHighlighted)
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = actBrush;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = inactBrush;
            }

            Canvas.SetLeft(ellipse, point.X - ellipse.ActualWidth / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.ActualHeight / 2);
        }

    }
}


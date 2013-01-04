using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;


namespace kinectfinal
{
    class Zoom : LevelAction
    {
        float zoomDistance;
        float exzoomDistance;

        public Zoom()
        {
            pointInUse = new bool[3] { true, true, false };
        }

        public override void confirm( )
        {
            if (turn == Turn.none)
            {
                if (rightHand.Position.Y > spine.Position.Y && leftHand.Position.Y > spine.Position.Y &&                        //在胸前
                     App.compareValue(rightHand.Position.Z, leftHand.Position.Z, 0.1f) &&                                            //在同一平面
                     App.comparePoint(rightHand.Position, preRighthand, 0.01f) && App.comparePoint(leftHand.Position, preLefthand, 0.01f)) //静止
                {
                    if (startCount < 8)
                    {
                        startCount++; //正在初始确认
                        state = "zoom confirming" + startCount;
                    }
                    else
                    {
                        turn = Turn.zoom;
                        startCount = 0; //完成初始确认
                        state = "zoom confirmed";
                        handLevelZ = (rightHand.Position.Z + leftHand.Position.Z) / 2;
                    }
                }
                else
                {
                    startCount = 0; //初始确认重置
                    state = "";
                }
            }
        }

        public override void process( )
        {
            ////////////缩放阶段//////////////////
            if ((preLefthand.Y - leftHand.Position.Y > 0.012 && preRighthand.Y - rightHand.Position.Y > 0.012)
                      || (Math.Abs(preLefthand.Z - leftHand.Position.Z) > 0.012 && Math.Abs(preRighthand.Z - rightHand.Position.Z) > 0.012))
            {
                state = "zoom pausing(too fast)";
                Times = 0;
            }

            else if (App.compareValue(rightHand.Position.Z, leftHand.Position.Z, 0.15f) &&                     //在同一平面
                     rightHand.Position.Y > spine.Position.Y && leftHand.Position.Y > spine.Position.Y)   //在胸前
            {
                if (App.compareValue(handLevelZ, rightHand.Position.Z, 0.12f) && App.compareValue(handLevelZ, leftHand.Position.Z, 0.12f)) //双手同在初始平面
                {
                    zoomDistance = rightHand.Position.X - leftHand.Position.X;
                    exzoomDistance = preRighthand.X - preLefthand.X;
                    Delta = zoomDistance - exzoomDistance;
                    Times = Times + Delta * 30;

                    main.Distance.Content = zoomDistance;
                    main.Delta.Content = Delta;
                    main.Times.Content = Times;

                    if (zoomDistance>0.8)
                    {
                        for (keepDo = 2; keepDo > 0; keepDo--)
                        {
                            state = "keep zooming+";
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0, 0); //按下CTRL鍵。　　
                            App.mouse_event(2048, 0, 0, 120, UIntPtr.Zero);//滑轮向上滚动
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0x2, 0);//放開CTRL鍵
                         }
                        Times = 0;
                    }
                    else if (zoomDistance < 0.2)
                    {
                        for (keepDo= 2; keepDo > 0; keepDo--)
                        {
                            state = "keep zooming-";
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0, 0); //按下CTRL鍵。　　
                            App.mouse_event(2048, 0, 0, -120, UIntPtr.Zero);//滑轮向下滚动
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0x2, 0);//放開CTRL鍵
                        }
                        Times = 0;
                    }
                    else
                    {
                        for (; Times >= 1; Times--)
                        {
                            state = "zooming+++++";
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0, 0); //按下CTRL鍵。　　
                            App.mouse_event(2048, 0, 0, 120, UIntPtr.Zero);//滑轮向上滚动
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0x2, 0);//放開CTRL鍵
                        }

                        for (; Times <= -1; Times++)
                        {
                            state = "zooming-----";
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0, 0); //按下CTRL鍵。　　
                            App.mouse_event(2048, 0, 0, -120, UIntPtr.Zero);//滑轮向下滚动
                            App.keybd_event(162, App.MapVirtualKey(162, 0), 0x2, 0);//放開CTRL鍵
                        }
                    }
                }
                else
                {
                   state = "zoom pausing(out of z)";
                   Times = 0;
                }

            }
            /////////结束此次或此组缩放/////////////
            else
            {
               state= "";
               turn = Turn.none;
               Times = 0;
            }
        }
      
    }
}

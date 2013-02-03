using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace kinectfinal
{
    class SwitchOver : LevelAction
    {
        float zoomDistance;
        float exzoomDistance;

        public SwitchOver()
        {
            pointInUse = new bool[3] { true, true, false };
        }

        public override void confirm( )
        {
            if (turn == Turn.none)
            {
                if (leftHand.Position.Y > spine.Position.Y && rightHand.Position.Y < spine.Position.Y &&                        //在胸前
                     App.compareValue(rightHand.Position.Z, leftHand.Position.Z, 0.1f) &&                                            //在同一平面
                     App.comparePoint(rightHand.Position, preRighthand, 0.01f) && App.comparePoint(leftHand.Position, preLefthand, 0.01f)) //静止
                {
                    if (startCount < 8)
                    {
                        startCount++; //正在初始确认
                        state = "switchover confirming" + startCount;
                    }
                    else
                    {
                        turn = Turn.switchover;
                        startCount = 0; //完成初始确认
                        state = "switchover confirmed";
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
            ////////////切换阶段//////////////////
            if ((preLefthand.Y - leftHand.Position.Y > 0.012 && preRighthand.Y - rightHand.Position.Y > 0.012)
                      || (Math.Abs(preLefthand.Z - leftHand.Position.Z) > 0.012 && Math.Abs(preRighthand.Z - rightHand.Position.Z) > 0.012))
            {
                state = "swichover pausing(too fast)";
                Times = 0;
            }

            else if (App.compareValue(rightHand.Position.Z, leftHand.Position.Z, 0.15f) &&                     //在同一平面
                     leftHand.Position.Y > spine.Position.Y && rightHand.Position.Y < spine.Position.Y)   //在胸前
            {
                if (App.compareValue(handLevelZ, rightHand.Position.Z, 0.12f) && App.compareValue(handLevelZ, leftHand.Position.Z, 0.12f)) //双手同在初始平面
                {
                    zoomDistance = rightHand.Position.X - spine.Position.X;
                    exzoomDistance = preRighthand.X - spine.Position.X;
                    Delta = zoomDistance - exzoomDistance;
                    Times = Times + Delta * 30;

                    main.Distance.Content = zoomDistance;
                    main.Delta.Content = Delta;
                    main.Times.Content = Times;
          
                    if (zoomDistance < 0.2)
                    {
                        for (keepDo= 2; keepDo > 0; keepDo--)
                        {
                            state = "keep switching-";
                            App.keybd_event(91, App.MapVirtualKey(91, 0), 0, 0); //按下Win鍵。　　
                            App.keybd_event(9, App.MapVirtualKey(9, 0), 0, 0); //按下Tab鍵。　                          
                        }
                        Times = 0;
                    }
                    else
                    {
                        for (; Times >= 1; Times--)
                        {
                            state = "switching+++++";
                            App.keybd_event(91, App.MapVirtualKey(91, 0), 0, 0); //按下Win鍵。　　
                            App.keybd_event(9, App.MapVirtualKey(9, 0), 0, 0); //按下Tab鍵。
                        }

                        for (; Times <= -1; Times++)
                        {
                            state = "switching-----";
                            App.keybd_event(91, App.MapVirtualKey(91, 0), 0, 0); //按下Win鍵。　　
                            App.keybd_event(9, App.MapVirtualKey(9, 0), 0, 0); //按下Tab鍵。
                        }
                    }
                }
                else
                {
                    state = "switching pausing(out of z)";
                   Times = 0;
                }

            }
            /////////结束此次或此组切换/////////////
            else
            {
               App.keybd_event(91, App.MapVirtualKey(91, 0), 0x2, 0);//放開Win鍵
               state= "";
               turn = Turn.none;
               Times = 0;
            }
        }
      
    }
}


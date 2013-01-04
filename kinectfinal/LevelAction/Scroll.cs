using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace kinectfinal
{
    class Scroll : LevelAction
    {
        public Scroll()
        {
            pointInUse=new bool[3]{false,true,false};
        }

        public override void confirm( )
        {
            if (turn == Turn.none)
            {
                if (leftHand.Position.Y < spine.Position.Y && rightHand.Position.Y > spine.Position.Y
                    && App.comparePoint(rightHand.Position, preRighthand, 0.01f))
                {
                    if (startCount < 8)
                    {
                        startCount++; //正在初始确认
                        state = "scroll confirming" + startCount;
                    }
                    else
                    {
                        turn = Turn.scroll;
                        startCount = 0; //完成初始确认
                        state = "scroll confirmed";
                        handLevelZ = rightHand.Position.Z;
                        handLevelX = rightHand.Position.X;
                        handLevelY = rightHand.Position.Y;
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
            if (Math.Abs(preRighthand.Z - rightHand.Position.Z) > 0.012 || Math.Abs(preRighthand.X - rightHand.Position.X) > 0.012)
            {
                state = "scroll pausing(too fast)";
                Times = 0;
            }
            else if (App.compareValue(handLevelX, rightHand.Position.X, 0.15f)   //in same vertical line
                       && rightHand.Position.Y > (spine.Position.Y-0.08) && leftHand.Position.Y < spine.Position.Y) //under and above the spine
            {
                if (App.compareValue(handLevelZ, rightHand.Position.Z, 0.11f))
                {
                    Delta = rightHand.Position.Y - preRighthand.Y;
                    Times = Times + Delta * 25;

                    main.Delta.Content = Delta;
                    main.Times.Content = Times;
                    if (rightHand.Position.Y - handLevelY > 0.12)
                    {
                        if (keepDo ==2)
                        {
                            keepDo = 0;
                            state = "keep scrolling+";　　
                            App.mouse_event(2048, 0, 0, 120, UIntPtr.Zero);//滑轮向上滚动
                        }
                        keepDo++;
                        Times = 0;
                    }
                    else if (handLevelY - rightHand.Position.Y > 0.22)
                    {
                        if (keepDo ==2)
                        {
                            keepDo = 0;
                            state = "keep scrolling-";
                            App.mouse_event(2048, 0, 0, -120, UIntPtr.Zero);//滑轮向下滚动
                        }
                        keepDo++;
                        Times = 0;
                    }
                    else
                    {
                        for (; Times >= 1; Times--)
                        {
                            state = "scrolling+++++";
                            App.mouse_event(2048, 0, 0, 120, UIntPtr.Zero);//滑轮向上滚动
                        }

                        for (; Times <= -1; Times++)
                        {
                            state = "scrolling-----";
                            App.mouse_event(2048, 0, 0, -120, UIntPtr.Zero);//滑轮向下滚动
                        }
                    }
                }
                else
                {
                    state = "scroll pausing(out of z)";
                    Times = 0;
                }
            }
            else
            {
                state = "";
                turn = Turn.none;
                Times = 0;
            }
        }

    }      
}

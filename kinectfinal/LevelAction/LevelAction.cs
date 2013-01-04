using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace kinectfinal
{
    abstract class LevelAction
    { 
        //show function in use
        public enum Turn { none, scroll, zoom };
        public static Turn turn { get; set; }

        //index to the mainwindow
        public static MainWindow main { get; set; }

        //initial level
        protected float handLevelZ;
        protected float handLevelX;
        protected float handLevelY;

        //content to display
        public static string state { get; set; }
        public int startCount { get; set; }

        //intermediate data
        protected int keepDo;
        protected float Delta;
        protected float Times;

        //point used in level action
        public static Joint leftHand = new Joint ();
        public static Joint  rightHand = new Joint();
        public static Joint  spine = new Joint();
        public static SkeletonPoint preLefthand = new SkeletonPoint();
        public static SkeletonPoint preRighthand = new SkeletonPoint();

        //registy for points to use
        public bool[] pointInUse = new bool[3];   //left right spine

        public abstract void confirm( );

        public abstract void process( );

    }
}

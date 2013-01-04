using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinectfinal
{
    class ActionPool
    {
        public Dictionary<LevelAction.Turn, LevelAction> levelActionList=new Dictionary<LevelAction.Turn,LevelAction>();

        public ActionPool()
        {
            levelActionList.Add(key: LevelAction.Turn.scroll, value: new Scroll());
            levelActionList.Add(key: LevelAction.Turn.zoom, value: new Zoom());
        }
    }
}

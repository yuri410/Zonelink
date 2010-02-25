using System;
using System.Collections.Generic;
using System.Text;
using XF = Microsoft.Xna.Framework;

namespace Code2015.LevelSystem
{
    class TaskManager
    {

    }

    enum TaskType
    {
        FloodDisaster

    }

    class Task
    {
        Dictionary<XF.PlayerIndex, float> points;

        public Task()
        {
            points = new Dictionary<XF.PlayerIndex, float>(5);

            points.Add(XF.PlayerIndex.One, 0);
            points.Add(XF.PlayerIndex.Two, 0);
            points.Add(XF.PlayerIndex.Three, 0);
            points.Add(XF.PlayerIndex.Four, 0);

        }

        public float GetContribution(XF.PlayerIndex player)
        {
            return points[player];
        }

        public void Contribute(XF.PlayerIndex player, float score)
        {
            points[player] += score;
        }




        public string Name
        {
            get;
            private set;
        }

        public TaskType Type
        {
            get;
            private set;
        }

    }
}

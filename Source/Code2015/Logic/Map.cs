using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.Logic
{
    public class Map
    {
        SimulationRegion region;
        PathFinderManager pathFinder;


        public Map(SimulationRegion region)
        {
            this.region = region;

            FileLocation fl = FileSystem.Instance.Locate("grad.bit", GameFileLocs.Nature);

            BitTable gradMap = new BitTable(32);
            gradMap.Load(fl);

            pathFinder = new PathFinderManager(gradMap);
        }

        public PathFinderManager PathFinder
        {
            get { return pathFinder; }
        }

        public void BlockArea(float lng, float lat, float r)
        {

        }

    }
}

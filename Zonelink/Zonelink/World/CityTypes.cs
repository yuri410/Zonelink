using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Code2015.World;

namespace Zonelink.World
{
    class GatherCity : City
    {
        Harvester harvester;
        NatureResource exRes;

        int resourceIndex = 0;
        float resourceBuffer;

        //资源搜索范围
        float gatherDistance;

        private List<NatureResource> nearResource = new List<NatureResource>();

        public NatureResource ExResource
        {
            get
            {
                if (nearResource.Count == 0)
                    return null;
                return nearResource[resourceIndex];
            }
        }


        public GatherCity(BattleField btfld, Player owner)
            : base(btfld, owner)
        {
            harvester = new Harvester(this, btfld.Map);
            harvester.GotHome += Harv_Home;
            harvester.GotThere += Harv_Dest;

            Harvester.Props hprop = getHarvProps();
            harvester.SetProps(hprop);

            if (this.Type == CityType.Oil)
            {
                gatherDistance = RulesTable.OilGatherDistance;
            }
            else if (this.Type == CityType.Green)
            {
                gatherDistance = RulesTable.GreenGatherDistance;
            }

        }

        public void SetTargetExResource(NatureResource res)
        {
            exRes = res;
        }

        public Harvester.Props getHarvProps()
        {
            Harvester.Props props = new Harvester.Props();
            props.HP = RulesTable.GreenHarvHP;
            props.Speed = RulesTable.GreenHarvSpeed;
            props.Storage = RulesTable.GreenHarvStorage;
            return props;
        }

        void Harv_Dest(object sender, EventArgs e)
        {
            harvester.Move(this.Longitude, this.Latitude);
            harvester.SetMovePurpose(MovePurpose.Home);

            // 自动返回
            //harvBackWait = getHarvWaitTime();
        }
        void Harv_Home(object sender, EventArgs e)
        {
            harvester.Move(exRes.Longitude, exRes.Latitude);
            harvester.SetMovePurpose(MovePurpose.Gather);

            if (!harvester.IsFullLoaded)
            {
                FindNewNaturalResource();
            }

            harvester.ExRes = exRes;


            //harvSendWait = getHarvWaitTime();
        }


        public override void UpdateResource(GameTime gameTime)
        {
            //开采资源
            //if (this.nearResource.Count > 0)
            //{
            //    float take = nearResource[resourceIndex].Exploit(10);
            //    if (take < 10)
            //    {
            //        FindNewNaturalResource();
            //    }
            //    resourceBuffer += take;
            //}

            harvester.Update(gameTime);

        }

        public override bool CanProduceRBall()
        {
            return this.resourceBuffer > RulesTable.RBallProduceBall;
        }

        //产生小球
        public override void ProduceBall()
        {
            this.battleField.CreateResourceBall(this);
            resourceBuffer -= RulesTable.RBallProduceBall;
        }


        //周围资源资源
        public void FindResources(List<NatureResource> resList)
        {
            for (int i = 0; i < resList.Count; i++)
            {
                float d = Vector3.Distance(resList[i].Position, this.Position);
                if (d < gatherDistance)
                {
                    nearResource.Add(resList[i]);
                }

            }
            nearResource.Sort(Camparision);
        }

        private int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, b.Position);
            float db = Vector3.DistanceSquared(b.Position, b.Position);
            return da.CompareTo(db);
        }


        //寻找新的资源
        private void FindNewNaturalResource()
        {
            for (int i = 0; i < nearResource.Count; i++)
            {
                if (nearResource[i].CurrentAmount > 0)
                    resourceIndex = i;
            }
        }

        public void NotifyGotResource(float change)
        {
            this.resourceBuffer += change;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
           

}

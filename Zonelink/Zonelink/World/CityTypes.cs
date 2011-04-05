using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Code2015.World;

namespace Zonelink.World
{
    abstract class GatherCity : City
    {
        Harvester harvester;
        NatureResource exRes;



        public GatherCity(BattleField btfld, Player owner)
            : base(btfld, owner)
        {
            harvester = new Harvester(this, btfld.Map);
            harvester.GotHome += Harv_Home;
            harvester.GotThere += Harv_Dest;

            Harvester.Props hprop = getHarvProps();
            harvester.SetProps(hprop);
        }

        public void SetTargetExResource(NatureResource res)
        {
            exRes = res;
            
        }

        public abstract void NotifyGotResource(float res);

        protected abstract Harvester.Props getHarvProps();

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
            harvester.ExRes = exRes;
            //harvSendWait = getHarvWaitTime();
        }
       

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            harvester.Update(dt);


        }
    }
    class OilCity : GatherCity
    {
        //public static readonly float GatherDistance = RulesTable.OilGatherDistance;
        int oilIndex = 0;
        float oilBuffer;

        private List<NatureResource> nearOil = new List<NatureResource>();


        public OilCity(BattleField btfld, Player owner)
            : base(btfld, owner)
        {

        }

        public override void NotifyGotResource(float res)
        {
            oilBuffer += res;
        }
        protected override Harvester.Props getHarvProps()
        {
            Harvester.Props props = new Harvester.Props();
            props.HP = RulesTable.OilHarvHP;
            props.Speed = RulesTable.OilHarvSpeed;
            props.Storage = RulesTable.OilHarvStorage;
            return props;
        }
        
        void FindNewOil()
        {
            for (int i = 0; i < nearOil.Count; i++)
            {
                if (nearOil[i].CurrentAmount > 0)
                    oilIndex = i;
            }
        }



        int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }

        public void FindResources(List<NatureResource> resList)
        {
            for (int i = 0; i < resList.Count; i++)
            {
                float d = Vector3.Distance(resList[i].Position, this.Position);
                if (d < RulesTable.OilGatherDistance)
                {
                    if (resList[i].Type == NatureResourceType.Oil)
                    {
                        nearOil.Add(resList[i]);
                    }

                }
            }
            nearOil.Sort(Camparision);
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            // 资源足够就造一个球
        }
    }

    class GreenCity : GatherCity
    {


        int woodIndex = 0;

        float woodBuffer;
        //周围自然资源
        private List<NatureResource> nearWood = new List<NatureResource>();

        public override void NotifyGotResource(float res)
        {
            woodBuffer += res;
        }
        protected override Harvester.Props getHarvProps()
        {
            Harvester.Props props = new Harvester.Props();
            props.HP = RulesTable.GreenHarvHP;
            props.Speed = RulesTable.GreenHarvSpeed;
            props.Storage = RulesTable.GreenHarvStorage;
            return props;
        }
        public GreenCity(BattleField btfld, Player owner)
            : base(btfld, owner)
        {

        }



        public NatureResource ExWood
        {
            get
            {
                if (nearWood.Count == 0)
                    return null;
                return nearWood[woodIndex];
            }
        }


        void FindNewWood()
        {
            for (int i = 0; i < nearWood.Count; i++)
            {
                if (nearWood[i].CurrentAmount > 0)
                    woodIndex = i;
            }
        }

        int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }

        public void FindResources(List<NatureResource> resList)
        {
            for (int i = 0; i < resList.Count; i++)
            {
                float d = Vector3.Distance(resList[i].Position, this.Position);
                if (d < RulesTable.GreenGatherDistance)
                {
                    if (resList[i].Type == NatureResourceType.Wood)
                    {
                        nearWood.Add(resList[i]);
                    }
                }
            }
            nearWood.Sort(Camparision);
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            // 资源足够就造一个球
        }
    }

    class NeutralCity : City 
    {

        public NeutralCity(BattleField btfld, Player owner)
            : base(btfld, owner)
        {

        }

    }
}

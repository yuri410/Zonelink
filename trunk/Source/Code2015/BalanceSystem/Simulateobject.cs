using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public class SimulateObject :IConfigurable, IUpdatable
    {
        /// <summary>
        /// 经度
        /// </summary>
        public float Longitude
        {
            get;
            private set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Latitude
        {
            get;
            private set;
        }

        /// <summary>
        ///  碳元素的改变量，该值仅由CarbonProduceSpeed决定
        /// </summary>
        float carbonChange;
        

        /// <summary>
        ///  碳元素的排放速度
        /// </summary>
        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }
        public float GetCarbonChange()
        {
            float r = carbonChange;
            carbonChange = 0;
            return r;
        }

        //#region unk
        ///// <summary>
        ///// 高能
        ///// </summary>
        //public float InitHPAmount
        //{
        //    get;
        //    set;
        //}
        //public float RemainingHPAmount
        //{
        //    get;
        //    set;
        //}
        //public float ConsumeHPSpeed
        //{
        //    get;
        //    set;
        //}


        ///// <summary>
        ///// 低能
        ///// </summary>
        //public float InitLPAmount
        //{
        //    get;
        //    set;
        //}
        //public float RemainingLPAmount
        //{
        //    get;
        //    set;
        //}
        //public float ConsumeLPSpeed
        //{
        //    get;
        //    set;
        //}
        //#endregion

        public SimulateRegion Region
        {
            get;
            private set;
        }

        public SimulateObject(SimulateRegion region)
        {
            Region = region;
        }
       
        /// <summary>
        ///  派生类先更新
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;
            this.carbonChange = this.CarbonProduceSpeed * hours;
        }
        #region IConfigurable 成员

    
        public virtual void Parse(ConfigurationSection sect)
        {
            Longitude = sect.GetSingle("Longitude");
            Latitude = sect.GetSingle("Latitude");
        }
        #endregion
    }
}

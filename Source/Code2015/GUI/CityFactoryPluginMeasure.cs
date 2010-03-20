using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{

    class CityFactoryPluginMeasure
    {
        City current;

        RenderSystem renderSys;

        FastList<CityPlugin> woodFactory = new FastList<CityPlugin>();
        FastList<CityPlugin> oilRefinary = new FastList<CityPlugin>();
        FastList<CityPlugin> school = new FastList<CityPlugin>();
        FastList<CityPlugin> hospital = new FastList<CityPlugin>();

        public City Current
        {
            get { return current; }
            set
            {
                if (!object.ReferenceEquals(current, value))
                {
                    current = value;

                    UpdateInfo();
                }
            }
        }

        public void UpdateInfo()
        {
            woodFactory.Clear();
            oilRefinary.Clear();
            school.Clear();
            hospital.Clear();

            if (current != null)
            {
                for (int i = 0; i < current.PluginCount; i++)
                {
                    switch (current[i].TypeId)
                    {
                        case CityPluginTypeId.WoodFactory:
                            woodFactory.Add(current[i]);
                            break;
                        case CityPluginTypeId.OilRefinary:
                            oilRefinary.Add(current[i]);
                            break;
                        case CityPluginTypeId.BiofuelFactory:
                            oilRefinary.Add(current[i]);
                            break;
                        case CityPluginTypeId.EducationOrg:
                            school.Add(current[i]);
                            break;
                        case CityPluginTypeId.Hospital:
                            hospital.Add(current[i]);
                            break;
                    }

                }
            }

            woodFactory.Trim();
            oilRefinary.Trim();
            school.Trim();
            hospital.Trim();
        }


        public bool HasWoodFactory
        {
            get { return woodFactory.Count > 0; }
        }
        public bool HasOilRefinary
        {
            get { return oilRefinary.Count > 0; }
        }

        public bool HasHospital
        {
            get { return hospital.Count > 0; }
        }

        public bool HasSchool
        {
            get { return school.Count > 0; }
        }

        public CityFactoryPluginMeasure(RenderSystem rs)
        {
            renderSys = rs;
        }


        public void RenderWoodFactory(Sprite sprite, Font font)
        {
            if (current != null)
            {
                int x = 675;
                if (!current.CanAddPlugins)
                {
                    x = 435;

                    font.DrawString(sprite, "No more wood factory can be built.",
                         x, 635, 13, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);
                }

                font.DrawString(sprite, "count: " + woodFactory.Count.ToString(),
                    x, 665, 12, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);

                float averE = 0;
                for (int i = 0; i < woodFactory.Count; i++)
                {
                    averE += woodFactory[i].LRPConvRate;
                }
                averE /= (float)woodFactory.Count;

                font.DrawString(sprite, "efficiency: " + averE.ToString(),
                    x, 685, 12, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);

            }
        }
        public void RenderOil(Sprite sprite, Font font)
        {
            if (current != null)
            {
                int x = 675;
                if (!current.CanAddPlugins)
                {
                    x = 435;

                    font.DrawString(sprite, "No more oil producer can be built.",
                         x, 635, 13, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);
                }

                font.DrawString(sprite, "count: " + oilRefinary.Count.ToString(),
                                   x, 665, 12, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);

                float averE = 0;
                for (int i = 0; i < oilRefinary.Count; i++)
                {
                    averE += oilRefinary[i].HRPConvRate;
                }
                averE /= (float)oilRefinary.Count;

                font.DrawString(sprite, "efficiency: " + averE.ToString(),
                    x, 685, 12, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);

            }
        }


        public void Update(GameTime time)
        {

        }
    }


}

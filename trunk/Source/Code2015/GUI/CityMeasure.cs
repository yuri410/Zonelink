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

    class CityMeasure
    {
        Texture upArrowR;
        Texture dnArrowR;
        Texture upArrowG;
        Texture dnArrowG;


        ValueSmoother populationDir;
        ValueSmoother diseaseDir;
        ValueSmoother devDir;

        ValueSmoother woodsup;
        ValueSmoother oilsup;
        ValueSmoother foodsup;

        float lastPopulation;
        float lastDisease;
        float lastDev;


        float devFlash;
        float popFlash;
        float disFlash;

        ProgressBar devBar;
        ProgressBar popBar;
        ProgressBar disBar;
        ProgressBar woodBar;
        ProgressBar oilBar;
        ProgressBar foodBar;


        City current;

        public City Current
        {
            get { return current; }
            set
            {
                if (!object.ReferenceEquals(current, value))
                {
                    current = value;
                    if (value != null)
                    {
                        populationDir.Clear();
                        diseaseDir.Clear();
                        devDir.Clear();

                        woodsup.Clear();
                        oilsup.Clear();
                        foodsup.Clear();

                        woodsup.Add(current.SelfLRCRatio);
                        oilsup.Add(current.SelfHRCRatio);
                        foodsup.Add(current.SelfFoodCostRatio);
                    }
                }
            }
        }

        public CityMeasure(RenderSystem rs)
        {
            populationDir = new ValueSmoother(10);
            diseaseDir = new ValueSmoother(10);
            devDir = new ValueSmoother(10);
            woodsup = new ValueSmoother(10);
            oilsup = new ValueSmoother(10);
            foodsup = new ValueSmoother(10);

            FileLocation fl = FileSystem.Instance.Locate("arr_up_red.tex", GameFileLocs.GUI);
            upArrowR = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("arr_down_red.tex", GameFileLocs.GUI);
            dnArrowR = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("arr_up_green.tex", GameFileLocs.GUI);
            upArrowG = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("arr_down_green.tex", GameFileLocs.GUI);
            dnArrowG = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_prgbar_imp.tex", GameFileLocs.GUI);
            Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);


            devBar = new ProgressBar();
            devBar.X = 610;
            devBar.Y = 689;
            devBar.Height = 18;
            devBar.Width = 117;
            devBar.ProgressImage = prgBg;
            devBar.Background = prgBg1;

            popBar = new ProgressBar();
            popBar.X = 610;
            popBar.Y = 654;
            popBar.Height = 18;
            popBar.Width = 117;
            popBar.ProgressImage = prgBg;
            popBar.Background = prgBg1;

            disBar = new ProgressBar();
            disBar.X = 610;
            disBar.Y = 721;
            disBar.Height = 18;
            disBar.Width = 117;
            disBar.ProgressImage = prgBg;
            disBar.Background = prgBg1;

            woodBar = new ProgressBar();
            woodBar.X = 444;
            woodBar.Y = 654;
            woodBar.Height = 18;
            woodBar.Width = 117;
            woodBar.ProgressImage = prgBg;
            woodBar.Background = prgBg1;

            oilBar = new ProgressBar();
            oilBar.X = 444;
            oilBar.Y = 689;
            oilBar.Height = 18;
            oilBar.Width = 117;
            oilBar.ProgressImage = prgBg;
            oilBar.Background = prgBg1;

            foodBar = new ProgressBar();
            foodBar.X = 444;
            foodBar.Y = 721;
            foodBar.Height = 18;
            foodBar.Width = 117;
            foodBar.ProgressImage = prgBg;
            foodBar.Background = prgBg1;



        }

        public int PopulationDirective
        {
            get;
            private set;
        }

        public int DiseaseDirective
        {
            get;
            private set;
        }
        public int DevelopmentDirective
        {
            get;
            private set;
        }

        public float Development
        {
            get;
            private set;
        }
        public float Disease
        {
            get;
            private set;
        }
        public float Population
        {
            get;
            private set;
        }

        int ClassifyDir(float v)
        {
            v *= 100;
            int sig = Math.Sign(v);
            int result = 0;
            v = Math.Abs(v);
            if (v > float.Epsilon)
            {
                if (v > 1)
                {
                    if (v > 2)
                    {
                        result = v > 3 ? 4 : 3;
                        result *= sig;
                        return result;
                    }
                    else
                    {
                        result = 2;
                        result *= sig;
                        return result;
                    }
                }
                else
                {
                    result = 1;
                    result *= sig;
                    return result;
                }
            }
            return result;
        }

        public void Update(GameTime time)
        {
            if (current != null)
            {
                float dev = current.Development / CityGrade.GetUpgradePoint(current.Size);
                float pop = current.Population / CityGrade.GetRefPopulation(current.Size);
                float dis = current.Disease / 2;
                Development = MathEx.Saturate(dev);
                Population = MathEx.Saturate(pop);
                Disease = MathEx.Saturate(dis);


                populationDir.Add((pop - lastPopulation) * 100);
                diseaseDir.Add(dis - lastDisease);
                devDir.Add(dev - lastDev);

                float v = populationDir.Result;
                PopulationDirective = ClassifyDir(v);
                v = diseaseDir.Result;
                DiseaseDirective = ClassifyDir(v);
                v = devDir.Result;
                DevelopmentDirective = ClassifyDir(v);

                lastPopulation = pop;
                lastDisease = dis;
                lastDev = dev;


                woodsup.Add(current.SelfLRCRatio);
                oilsup.Add(current.SelfHRCRatio);
                foodsup.Add(current.SelfFoodCostRatio);

            }
        }

        public void Render(Sprite sprite, Font alger)
        {
            if (current != null)
            {
                devBar.Value = Development;
                devBar.Render(sprite);

                popBar.Value = Population;
                popBar.Render(sprite);

                disBar.Value = Disease;
                disBar.Render(sprite);

                foodBar.Value = MathEx.Saturate(foodsup.Result);
                foodBar.Render(sprite);

                oilBar.Value = MathEx.Saturate(oilsup.Result);
                oilBar.Render(sprite);

                woodBar.Value = MathEx.Saturate(woodsup.Result);
                woodBar.Render(sprite);

                if (PopulationDirective < 0)
                {
                    //Rectangle rect;

                    //rect.Y = 650;
                    //rect.Width = 16;
                    //rect.Height = 16;
                    //for (int i = PopulationDirective; i < 0; i++)
                    //{
                    //    rect.X = 620 + i * 16;
                    //    sprite.Draw(leftArrowR, rect, ColorValue.White);
                    //}
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(popFlash) + 1));
                    popFlash += PopulationDirective * 0.2f;
                    sprite.Draw(dnArrowR, 733, 650, c);
                }
                else if (PopulationDirective > 0)
                {
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(popFlash) + 1));

                    popFlash += PopulationDirective * 0.2f;
                    sprite.Draw(upArrowG, 733, 650, c);
                }

                if (DevelopmentDirective > 0)
                {
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(devFlash) + 1));
                    devFlash += DevelopmentDirective * 0.2f;
                    sprite.Draw(upArrowG, 733, 685, c);
                }
                else if (DevelopmentDirective < 0)
                {
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(devFlash) + 1));
                    devFlash += DevelopmentDirective * 0.2f;
                    sprite.Draw(dnArrowR, 733, 685, c);
                }

                if (DiseaseDirective > 0)
                {
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(disFlash) + 1));
                    disFlash += DiseaseDirective * 0.2f;
                    sprite.Draw(upArrowR, 733, 718, c);
                }
                else if (DiseaseDirective < 0)
                {
                    ColorValue c = ColorValue.White;
                    c.A = (byte)(byte.MaxValue * 0.5 * (Math.Cos(disFlash) + 1));
                    disFlash += DiseaseDirective * 0.2f;
                    sprite.Draw(dnArrowG, 733, 718, c);
                }

                if (disFlash > 2 * MathEx.PIf)
                    disFlash -= 2 * MathEx.PIf;
                else if (disFlash < -2 * MathEx.PIf)
                    disFlash += 2 * MathEx.PIf;

                if (devFlash > 2 * MathEx.PIf)
                    devFlash -= 2 * MathEx.PIf;
                else if (devFlash < -2 * MathEx.PIf)
                    devFlash += 2 * MathEx.PIf;

                if (popFlash > 2 * MathEx.PIf)
                    popFlash -= 2 * MathEx.PIf;
                else if (popFlash < -2 * MathEx.PIf)
                    popFlash += 2 * MathEx.PIf;






                alger.DrawString(sprite, "Wood", 428, 638, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
                alger.DrawString(sprite, "Oil", 428, 673, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
                alger.DrawString(sprite, "Food", 428, 705, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
                alger.DrawString(sprite, "Population", 594, 638, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
                alger.DrawString(sprite, "Development", 594, 673, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
                alger.DrawString(sprite, "Disease", 594, 705, 12, DrawTextFormat.Left, (int)ColorValue.Brown.PackedValue);
            }
        }
    }
}

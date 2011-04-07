/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Code2015.EngineEx;
using Zonelink;
using Zonelink.World;

namespace Apoc3D
{
    unsafe static class Utils
    {
        public static string[] EmptyStringArray
        {
            get;
            private set;
        }

        static Utils()
        {
            EmptyStringArray = new string[0];
        }

        public static GameConfiguration LoadConfig(string configName)
        {
            string path = Path.Combine(GameFileLocs.Configs, configName);
            GameConfiguration resCon = new GameConfiguration(path);
            
            return resCon;
        }

        public static float GetRBallContribution(RBallType type)
        {
            switch (type)
            {
                case RBallType.Disease:
                    return RulesTable.DiseaseBallContribution;

                case RBallType.Health:
                    return RulesTable.HealthBallContribution;

                case RBallType.Education:
                    return RulesTable.EducationBallContribution;

                case RBallType.Oil:
                    return RulesTable.OilBallContribution;

                case RBallType.Green:
                    return RulesTable.GreenBallContribution;

                case RBallType.Volience:
                    return RulesTable.VolienceBallContribution;

               default:
                    throw new InvalidOperationException();

            }
        }

        public static RBallType GetProductionType(CityType type)
        {
            switch (type)
            {
                case CityType.Health:
                    return RBallType.Health;
                case CityType.Volience:
                    return RBallType.Volience;
                case CityType.Disease:
                    return RBallType.Disease;
                case CityType.Education:
                    return RBallType.Education;
                case CityType.Neutral:
                    throw new InvalidOperationException();
                case CityType.Oil:
                    return RBallType.Oil;
                case CityType.Green:

                    return RBallType.Green;
            } throw new InvalidOperationException();
        }

        //public static string GetTempFileName() { throw new NotImplementedException(); }
    }
}

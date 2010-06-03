/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

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
using System.Text;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  联合国八大问题
    /// </summary>
    public enum MdgType
    {
        /// <summary>
        ///  Eradicate extreme hunger and poverty
        /// </summary>
        Hunger = 0,
        /// <summary>
        ///  Achieve universal primary education
        /// </summary>
        Education = 1,
        /// <summary>
        ///  Promote gender equality and empower women
        /// </summary>
        GenderEquality = 2,
        /// <summary>
        ///  Reduce child mortality
        /// </summary>
        ChildMortality = 3,
        /// <summary>
        ///  Improve maternal health
        /// </summary>
        MaternalHealth = 4,
        /// <summary>
        ///  Combat HIV/AIDS, malaria and other diseases
        /// </summary>
        Diseases = 5,
        /// <summary>
        ///  Ensure environmental sustainability
        /// </summary>
        Environment = 6,
        /// <summary>
        ///  Develop a global partnership for development
        /// </summary>
        Partnership = 7,
        Count = 8
    }
}

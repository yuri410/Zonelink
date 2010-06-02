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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Logic;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示城市占领的状态
    /// </summary>
    public class CaptureState
    {

        public bool Changed
        {
            get;
            private set;
        }

        public void Serialize(ContentBinaryWriter bw)
        {
            
            Changed = false;
        }
        public void Deserialize(ContentBinaryReader br)
        {
        }

        public bool IsCapturing
        {
            get { return NewOwner1 != null || NewOwner2 != null || NewOwner3 != null || NewOwner4 != null; }
        }

        /// <summary>
        ///  检查占领是否完成
        /// </summary>
        /// <returns>若完成，返回玩家</returns>
        public Player CheckCapture()
        {
            if (CaputreProgress1 >= 1)
            {
                return NewOwner1;
            }
            if (CaputreProgress2 >= 1)
            {
                return NewOwner2;
            }
            if (CaputreProgress3 >= 1)
            {
                return NewOwner3;
            }
            if (CaputreProgress4 >= 1)
            {
                return NewOwner4;
            }
            return null;
        }



        public City NearbyCity1
        {
            get;
            private set;
        }

        public City NearbyCity2
        {
            get;
            private set;
        }
        public City NearbyCity3
        {
            get;
            private set;
        }
        public City NearbyCity4
        {
            get;
            private set;

        }



        /// <summary>
        ///  即将占领的玩家
        /// </summary>
        public Player NewOwner1
        {
            get;
            private set;
        }
        public Player NewOwner2
        {
            get;
            private set;
        }
        public Player NewOwner3
        {
            get;
            private set;
        }
        public Player NewOwner4
        {
            get;
            private set;
        }
        public float Speed1
        {
            get;
            private set;
        }
        public float Speed2
        {
            get;
            private set;
        }
        public float Speed3
        {
            get;
            private set;
        }
        public float Speed4
        {
            get;
            private set;
        }

        /// <summary>
        ///  占领进度
        /// </summary>
        public float CaputreProgress1
        {
            get;
            private set;
        }
        public float CaputreProgress2
        {
            get;
            private set;
        }
        public float CaputreProgress3
        {
            get;
            private set;
        }
        public float CaputreProgress4
        {
            get;
            private set;
        }

        public void CancelCapture(Player player)
        {
            if (player == NewOwner1)
            {
                NewOwner1 = null;
                CaputreProgress1 = 0;
            }
            if (player == NewOwner2)
            {
                NewOwner2 = null;
                CaputreProgress2 = 0;
            }
            if (player == NewOwner3)
            {
                NewOwner3 = null;
                CaputreProgress3 = 0;
            }
            if (player == NewOwner4)
            {
                NewOwner4 = null;
                CaputreProgress4 = 0;
            }
            Changed = true;
        }
        public float GetCaptureProgress(Player player)
        {
            if (player == NewOwner1)
            {
                return CaputreProgress1;
            }
            if (player == NewOwner2)
            {
                return CaputreProgress2;
            }
            if (player == NewOwner3)
            {
                return CaputreProgress3;
            }
            if (player == NewOwner4)
            {
                return CaputreProgress4;
            }
            return 0;
        }
        public bool IsPlayerCapturing(Player player)
        {
            if (player == NewOwner1)
            {
                return true;
            }
            if (player == NewOwner2)
            {
                return true;
            }
            if (player == NewOwner3)
            {
                return true;
            }
            if (player == NewOwner4)
            {
                return true;
            }
            return false;
        }
        public float GetSpeed(Player player) 
        {
            if (player == NewOwner1)
            {
                return Speed1;
            }
            if (player == NewOwner2)
            {
                return Speed2;
            }
            if (player == NewOwner3)
            {
                return Speed3;
            }
            if (player == NewOwner4)
            {
                return Speed4;
            }
            return 0;
        }
        public bool CanCapture(Player player)
        {
            if (!IsCapturing)
                return true;
            if (player == NewOwner1)
            {
                return false;
            }
            if (player == NewOwner2)
            {
                return false;
            }
            if (player == NewOwner3)
            {
                return false;
            }
            if (player == NewOwner4)
            {
                return false;
            }
            return true;
        }
        public void SetCapture(Player player, City nearby)
        {
            if (object.ReferenceEquals(NewOwner1, null))
            {
                NewOwner1 = player;
                NearbyCity1 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner2, null))
            {
                NewOwner2 = player;
                NearbyCity2 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner3, null))
            {
                NewOwner3 = player;
                NearbyCity3 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner4, null))
            {
                NewOwner4 = player;
                NearbyCity4 = nearby;
            }
            Changed = true;
        }

        public void ReceiveGood(Player player, float hrAmount, float lrAmount)
        {
            if (player == NewOwner1)
            {
                Speed1 = hrAmount + lrAmount * 0.5f;
                CaputreProgress1 += Speed1;
            }
            else if (player == NewOwner2)
            {
                Speed2 = hrAmount + lrAmount * 0.5f;
                CaputreProgress2 += Speed2;
            }
            else if (player == NewOwner3)
            {
                Speed3 = hrAmount + lrAmount * 0.5f;
                CaputreProgress3 += Speed3;
            }
            else if (player == NewOwner4)
            {
                Speed4 = hrAmount + lrAmount * 0.5f;
                CaputreProgress4 += Speed4;
            }
        }
    }

}

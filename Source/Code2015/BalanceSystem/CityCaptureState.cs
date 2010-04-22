using System;
using System.Collections.Generic;
using System.Text;
using Code2015.Logic;
using Apoc3D.Vfs;

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

        }
        public void Deserialize(ContentBinaryReader br)
        {
            Changed = false;
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
                CaputreProgress1 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner2)
            {
                CaputreProgress2 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner3)
            {
                CaputreProgress3 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner4)
            {
                CaputreProgress4 += hrAmount + lrAmount * 0.5f;
            }
        }
    }

}

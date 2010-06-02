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

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一种资源的存储器。
    ///  可以储存一定量的资源。并且从这批资源中申请获得一定数量，以及将一定数量的资源存储进来
    /// </summary>
    /// <remarks>
    ///  生产有限，使用无限
    ///  
    ///  生产限制为能源转化率
    ///  使用限制使用“上限”实现
    /// </remarks>
    public class ResourceStorage
    {
        [SLGValue()]
        const float SafeLimitRate = 0.05f;
        [SLGValue()]
        const float StandardStorageBallanceRate = 0.1f;

        float amount;
        float limit;


        public ResourceStorage(float a, float limit)
        {
            this.amount = a;
            this.limit = limit;
        }

        /// <summary>
        ///  获取或设置当前资源数量
        /// </summary>
        public float Current
        {
            get { return amount; }
            private set { amount = value; }
        }

        /// <summary>
        ///  获取或设置资源存储量的上限
        /// </summary>
        public float MaxLimit
        {
            get { return limit; }
            set { limit = value; }
        }

        public float StandardStorageBalance
        {
            get { return limit * StandardStorageBallanceRate; }
        }
        public float SafeLimit
        {
            get { return limit * SafeLimitRate; }
        }

        /// <summary>
        ///  申请获得能源
        /// </summary>
        /// <param name="amount">要求的能源量</param>
        /// <returns>实际申请到的能源量</returns>
        public float Apply(float amount)
        {
            if (Current >= amount)
            {
                Current -= amount;
                return amount;
            }
            float r = Current;
            Current = 0;
            return r;
        }

        public float ApplyFar(float amount)
        {
            if (Current > SafeLimit)
            {
                return Apply(amount);
            }
            return 0;
        }

        /// <summary>
        ///  将过剩资源提交，存储起来
        /// </summary>
        /// <param name="amount">提交的数量</param>
        /// <returns>实际接受提交的数量</returns>
        public float Commit(float amount)
        {
            float r = Current + amount;
            if (r > MaxLimit)
            {
                r = MaxLimit - Current;
                Current = MaxLimit;
                return r;
            }
            Current = r;
            return amount;
        }
    }

}

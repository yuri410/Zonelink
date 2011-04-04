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
using System.ComponentModel;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Microsoft.Xna.Framework;

namespace Apoc3D.Scene
{
    /// <summary>
    /// 场景中的对象
    /// </summary>
    public abstract class SceneObject :  IDisposable
    {
        

        #region 字段

        /// <summary>
        ///  该物体的包围球
        /// </summary>
        public BoundingSphere BoundingSphere;

        /// <summary>
        ///  该物体在世界坐标系中的变换矩阵
        /// </summary>
        public Matrix Transformation = Matrix.Identity;

        #endregion

        #region 属性



        #endregion

        public abstract void Render();

        #region IUpdatable 成员

        /// <summary>
        ///  更新该物体的状态，每一帧如果可见，则被引擎调用
        /// </summary>
        /// <param name="dt">帧时间间隔，以秒为单位</param>
        public abstract void Update(GameTime dt);

        #endregion

        #region IDisposable 成员

        /// <summary>
        ///  释放场景物体所使用的所有资源。应在派生类中重写。
        /// </summary>
        /// <param name="disposing">表示是否需要释放该物体所引用的其他资源，当为真时，调用那些资源的Dispose方法</param>
        protected virtual void Dispose(bool disposing)
        {

        }

        /// <summary>
        ///  获取一个布尔值，表示该场景物体是否已经释放了资源。
        /// </summary>
        [Browsable(false)]
        public bool Disposed
        {
            get;
            private set;
        }

        /// <summary>
        ///  释放场景物体所使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}

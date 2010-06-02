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
using Apoc3D;
using Apoc3D.Vfs;
using Code2015.Logic;
using Code2015.World;
using Code2015.BalanceSystem;

namespace Code2015.Network
{
    public class StateDataBuffer
    {
        const int BufferSize = 1024;
        byte[] buffer = new byte[BufferSize];        

        MemoryStream stream;
        ContentBinaryReader reader;
        ContentBinaryWriter writer;

        int length;
        public int Length
        {
            get { return length; }
        }

        unsafe public StateDataBuffer()
        {
            fixed (byte* src = &buffer[0])
            {
                stream = new MemoryStream(src, BufferSize);
            }
            reader = new ContentBinaryReader(stream);
            writer = new ContentBinaryWriter(stream);
        }

        public void Reset() 
        {
            length = 0;
            stream.Position = 0;
        }
        public ContentBinaryReader Reader
        {
            get { return reader; }
        }
        public ContentBinaryWriter Writer 
        {
            get { return writer; }
        }

        public void EndWrite()
        {
            length = (int)stream.Position;
        }
    }

    class NetworkGameStateData
    {
        //Dictionary<string, ILogicStateObject> stateObjects = new Dictionary<string, ILogicStateObject>();
        Dictionary<string, StateDataBuffer> stateDataBuffer = new Dictionary<string, StateDataBuffer>();

        SimulationWorld world;

        public NetworkGameStateData(GameState state)
        {
            world = state.SLGWorld;
        }        

        public void SendState()
        {
            for (int i = 0; i < world.Count; i++)
            {

            }

        }

        public void ReceiveState()
        {

        }


    }
}

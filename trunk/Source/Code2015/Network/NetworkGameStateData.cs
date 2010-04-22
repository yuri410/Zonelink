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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.GUI;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.World.Screen
{
    /// <summary>
    ///  联合国八大问题
    /// </summary>
    enum MdgType
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

    /// <summary>
    ///  表示一个MDG拼图碎片
    /// </summary>
    class MdgPiece : UIComponent
    {
        const float Radius = 16;

        ScreenRigidBody body;

        /// <summary>
        ///  either 4,2 or 1
        /// </summary>
        int bitMask;

        MdgType type;

        Texture image;

        ScreenPhysicsWorld physicsWorld;
        MdgResourceManager manager;

        public MdgPiece(MdgResourceManager manager, ScreenPhysicsWorld world, MdgType type, int bitMask, Vector2 pos, float ori)
        {
            this.manager = manager;
            this.physicsWorld = world;

            this.body = new ScreenRigidBody();
            this.body.Orientation = ori;
            this.body.Position = pos;
            
            
            this.type = type;
            this.bitMask = bitMask;

            this.image = MdgResource.LoadImage(type, bitMask);
        }

        public int BitMask
        {
            get { return bitMask; }
        }

        public MdgType Type 
        {
            get { return type; }
        }

        public object Merge(MdgPiece other)
        {
            if (CheckMerge(other))
            {
                int bit = other.bitMask | bitMask;

                if (bit == 7)
                {
                    MdgResource res =  new MdgResource(physicsWorld, type, body.Position, body.Orientation);

                    manager.Remove(this);
                    manager.Remove(other);
                    manager.Add(res);
                    return res;
                }

                MdgPiece piece = new MdgPiece(manager, physicsWorld, type, bit, body.Position, body.Orientation);
                manager.Remove(this);
                manager.Remove(other); 
                manager.Add(piece);
                return piece;
            }
            return null;
        }

        public bool CheckMerge(MdgPiece other)
        {
            if (other.type == type)
            {
                return (other.bitMask | bitMask) > bitMask && (other.bitMask & bitMask) == 0;
            }
            return false;
        }

        public override void Render(Sprite sprite)
        {
            if (image != null)
            {
                sprite.SetTransform(Matrix.RotationZ(body.Orientation));
                sprite.Draw(image, 0, 0, ColorValue.White);
            }
        }

        //public override void Update(GameTime time)
        //{

        //}
    }

    /// <summary>
    ///  表示一个拼好的MDG图标
    /// </summary>
    class MdgResource : UIComponent
    {
        public static Texture LoadImage(MdgType type, int bitmask)
        {
            string suffix = string.Empty;

            switch (bitmask) 
            {
                case 1:
                    suffix = "_1";
                    break;
                case 2:
                    suffix = "_2";
                    break;
                case 3:
                    suffix = "_12";
                    break;
                case 4:
                    suffix = "_3";
                    break;
                case 5:
                    suffix = "_31";
                    break;
                case 6:
                    suffix = "_23";
                    break;
            }

            FileLocation fl = FileSystem.Instance.Locate("goal" + ((int)type).ToString() + suffix + ".tex", GameFileLocs.UI);
            return UITextureManager.Instance.CreateInstance(fl);
        }

        const float Radius = 32;

        ScreenRigidBody body;

        Texture image;

        MdgType type;
        ScreenPhysicsWorld physicsWorld;

        public MdgType Type
        {
            get { return type; }
        }


        public MdgResource(ScreenPhysicsWorld world, MdgType type, Vector2 pos, float ori)
        {
            this.physicsWorld = world;

            this.body = new ScreenRigidBody();
            this.body.Orientation = ori;
            this.body.Position = pos;

            this.type = type;
            this.image = LoadImage(type, 7);
        }

        public override void Render(Sprite sprite)
        {
            if (image != null)
            {
                Vector2 pos = body.Position;
                sprite.SetTransform(Matrix.RotationZ(body.Orientation) * Matrix.Translation(pos.X, pos.Y, 0));
                sprite.Draw(image, 0, 0, ColorValue.White);
            }
        }

        //public override void Update(GameTime time)
        //{
            
        //}
    }

    /// <summary>
    ///  对拼图游戏中的各种物体的管理器
    ///  记录这种物品的数量，并维护他们在物理引擎的“是否激活”状态
    /// </summary>
    class MdgResourceManager
    {
        /// <summary>
        ///  第一个索引为MdgType，第二个为bitmask，第三个为list index
        /// </summary>
        FastList<MdgPiece>[][] pieces;
        /// <summary>
        ///  第一个索引为MdgType
        /// </summary>
        FastList<MdgResource>[] balls;

        public MdgResourceManager()
        {
            pieces = new FastList<MdgPiece>[(int)MdgType.Count][];

            balls = new FastList<MdgResource>[(int)MdgType.Count];

            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i] = new FastList<MdgPiece>[8];

                pieces[i][0] = null;

                // 1
                pieces[i][1] = new FastList<MdgPiece>();

                // 2
                pieces[i][2] = new FastList<MdgPiece>();

                // 4
                pieces[i][4] = new FastList<MdgPiece>();

                // 1, 2
                pieces[i][3] = new FastList<MdgPiece>();

                // 1, 4
                pieces[i][5] = new FastList<MdgPiece>();

                // 2, 6
                pieces[i][6] = new FastList<MdgPiece>();

                // 1,2,4
                pieces[i][7] = new FastList<MdgPiece>();


                balls[i] = new FastList<MdgResource>();
            }

        }

        public void Add(MdgPiece piece)
        {
            pieces[(int)piece.Type][piece.BitMask].Add(piece);
        }
        public void Add(MdgResource res)
        {
            balls[(int)res.Type].Add(res);
        }
        public void Remove(MdgPiece piece)
        {
            pieces[(int)piece.Type][piece.BitMask].Remove(piece);
        }
        public void Remove(MdgResource res)
        {
            balls[(int)res.Type].Remove(res);
        }


        public int GetPieceCount(MdgType type, int bitmask)
        {
            return pieces[(int)type][bitmask].Count;
        }
        public MdgPiece GetPiece(MdgType type, int bitmask, int index)
        {
            return pieces[(int)type][bitmask][index];
        }

        public int GetResourceCount(MdgType type)
        {
            return balls[(int)type].Count;
        }
        public MdgResource GetResource(MdgType type, int index)
        {
            return balls[(int)type][index];
        }

            
            //public void Update(GameTime time)
            //{
                
            //}



    }

}

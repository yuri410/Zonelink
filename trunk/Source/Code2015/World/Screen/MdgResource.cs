using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.GUI;

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
        Hunger = 7,
        /// <summary>
        ///  Achieve universal primary education
        /// </summary>
        Education = 7 << 4,
        /// <summary>
        ///  Promote gender equality and empower women
        /// </summary>
        GenderEquality = 7 << 8,
        /// <summary>
        ///  Reduce child mortality
        /// </summary>
        ChildMortality = 7 << 12,
        /// <summary>
        ///  Improve maternal health
        /// </summary>
        MaternalHealth = 7 << 16,
        /// <summary>
        ///  Combat HIV/AIDS, malaria and other diseases
        /// </summary>
        Diseases = 7 << 20,
        /// <summary>
        ///  Ensure environmental sustainability
        /// </summary>
        Environment = 7 << 24,
        /// <summary>
        ///  Develop a global partnership for development
        /// </summary>
        Partnership = 7 << 28,
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

        public MdgPiece(MdgType type, int piece, Vector2 pos, float ori)
        {
            this.body = new ScreenRigidBody();
            this.body.Orientation = ori;
            this.body.Position = pos;

            this.type = type;
            this.bitMask = 1 << piece;

            this.image = MdgResource.LoadImage(type);
        }

        public bool CheckMerge(MdgPiece other)
        {
            if (other.type == type)
            {
                return (other.bitMask & bitMask) == (int)type && (other.bitMask & bitMask) == 0;
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

        public override void Update(GameTime time)
        {

        }
    }

    /// <summary>
    ///  表示一个拼好的MDG图标
    /// </summary>
    class MdgResource : UIComponent
    {
        public static Texture LoadImage(MdgType type)
        {
            switch (type)
            {
                case MdgType.ChildMortality:

                    break;
                case MdgType.Diseases:
                    break;
                case MdgType.Education:
                    break;
                case MdgType.Environment:
                    break;
                case MdgType.GenderEquality:
                    break;
                case MdgType.Hunger:
                    break;
                case MdgType.MaternalHealth:
                    break;
                case MdgType.Partnership:
                    break;
            } 
            return null;
        }

        const float Radius = 32;

        ScreenRigidBody body;

        Texture image;

        MdgType type;

        public MdgResource(MdgType type, Vector2 pos, float ori)
        {
            this.body = new ScreenRigidBody();
            this.body.Orientation = ori;
            this.body.Position = pos;

            this.type = type;
            this.image = LoadImage(type);
        }

        public override void Render(Sprite sprite)
        {
            if (image != null)
            {
                sprite.SetTransform(Matrix.RotationZ(body.Orientation));
                sprite.Draw(image, 0, 0, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            
        }
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
        FastList<MdgPiece>[][] peices;
        /// <summary>
        ///  第一个索引为MdgType
        /// </summary>
        FastList<MdgResource>[] balls;

        public MdgResourceManager()
        {
            peices = new FastList<MdgPiece>[(int)MdgType.Count][];

            balls = new FastList<MdgResource>[(int)MdgType.Count];

            for (int i = 0; i < peices.Length; i++)
            {
                peices[i][0] = null;

                // 1
                peices[i][1] = new FastList<MdgPiece>();

                // 2
                peices[i][2] = new FastList<MdgPiece>();

                // 4
                peices[i][4] = new FastList<MdgPiece>();

                // 1, 2
                peices[i][3] = new FastList<MdgPiece>();

                // 1, 4
                peices[i][5] = new FastList<MdgPiece>();

                // 2, 6
                peices[i][6] = new FastList<MdgPiece>();

                // 1,2,4
                peices[i][7] = new FastList<MdgPiece>();


                balls[i] = new FastList<MdgResource>();
            }

        }

        public int GetPieceCount(MdgType type, int bitmask)
        {
            return peices[(int)type][bitmask].Count;
        }
        public MdgPiece GetPiece(MdgType type, int bitmask, int index)
        {
            return peices[(int)type][bitmask][index];
        }

        public int GetResourceCount(MdgType type)
        {
            return balls[(int)type].Count;
        }
        public MdgResource GetResource(MdgType type, int index)
        {
            return balls[(int)type][index];
        }


        public void Update(GameTime time)
        {

        }

    }

}

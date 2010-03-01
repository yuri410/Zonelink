using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
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

    enum MdgIconType
    {
        Piece,
        Ball
    }

    static class MdgPhysicsParams
    {
        public const float PieceRadius = 32;
        public const float PieceMass = 1;
        public const float PieceElasity = 0.5f;
        public const float PieceFriction = 0.5f;
        public const float PieceAngularDamp = 0.5f;
        public const float PieceLinearDamp = 0.5f;

        public const float BallRadius = 32;
        public const float BallMass = 2.5f;
        public const float BallElasity = 0.5f;
        public const float BallFriction = 0.5f;
        public const float BallAngularDamp = 0.5f;
        public const float BallLinearDamp = 0.5f;

        public const float InactiveAlpha = 0.2f;
    }



    interface IMdgSelection
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        MdgIconType IconType { get; }

    }

    /// <summary>
    ///  表示一个MDG拼图碎片
    /// </summary>
    class MdgPiece : UIComponent, IMdgSelection
    {
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
            this.body.Radius = MdgPhysicsParams.PieceRadius;
            this.body.Mass = MdgPhysicsParams.PieceMass;
            this.body.Elasity = MdgPhysicsParams.PieceElasity;
            this.body.Friction = MdgPhysicsParams.PieceFriction;
            this.body.AngularDamp = MdgPhysicsParams.PieceAngularDamp;
            this.body.LinearDamp = MdgPhysicsParams.PieceLinearDamp;
            this.body.Tag = this;

            world.Add(body);

            this.type = type;
            this.bitMask = bitMask;

            this.image = MdgResource.LoadImage(type, bitMask);
        }

        public void NotifyRemoved()
        {
            physicsWorld.Remove(body);
        }

        public int BitMask
        {
            get { return bitMask; }
        }

        public MdgType Type
        {
            get { return type; }
        }

        public bool HitTest(int x, int y)
        {
            Vector2 pos = body.Position;

            float d = (float)Math.Sqrt(MathEx.Sqr(x - pos.X) + MathEx.Sqr(y - pos.Y));

            return d <= body.Radius;
        }
        public object Merge(MdgPiece other)
        {
            if (CheckMerge(other))
            {
                int bit = other.bitMask | bitMask;

                if (bit == 7)
                {
                    MdgResource res = new MdgResource(physicsWorld, type, body.Position, body.Orientation);

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
        public bool IsPrimary
        {
            get;
            set;
        }
        public override void Render(Sprite sprite)
        {
            if (image != null)
            {
                const float scaler = 0.21333f;

                Vector2 rectr = new Vector2(image.Width * scaler, image.Height * scaler);
                body.Radius = rectr.Length() * 0.5f;

                Vector2 pos = body.Position;
                float r = body.Radius;

                sprite.SetTransform(
                    Matrix.Scaling(scaler, scaler, 1) *
                    Matrix.Translation(-rectr.X * 0.5f, -rectr.Y * 0.5f, 0) * Matrix.RotationZ(-body.Orientation) * Matrix.Translation(pos.X, pos.Y, 0));

                if (IsPrimary)
                {
                    sprite.Draw(image, 0, 0, ColorValue.White);
                }
                else
                {
                    ColorValue opa = new ColorValue(MdgPhysicsParams.InactiveAlpha, 1, 1, 1);
                    sprite.Draw(image, 0, 0, opa);
                }
            }

        }

        //public override void Update(GameTime time)
        //{
        //}

        #region IMdgSelection 成员


        public MdgIconType IconType
        {
            get { return MdgIconType.Piece; }
        }

        public Vector2 Position
        {
            get { return body.Position; }
            set { body.Position = value; }
        }
        public Vector2 Velocity
        {
            get { return body.Velocity; }
            set { body.Velocity = value; }
        }
        #endregion
    }

    /// <summary>
    ///  表示一个拼好的MDG图标
    /// </summary>
    class MdgResource : UIComponent, IMdgSelection
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

            FileLocation fl = FileSystem.Instance.Locate("goal" + ((int)type + 1).ToString() + suffix + ".tex", GameFileLocs.GUI);
            return UITextureManager.Instance.CreateInstance(fl);
        }

        ScreenRigidBody body;

        Texture image;

        MdgType type;
        ScreenPhysicsWorld physicsWorld;

        public MdgType Type
        {
            get { return type; }
        }

        public bool HitTest(int x, int y)
        {
            Vector2 pos = body.Position;

            float d = (float)Math.Sqrt(MathEx.Sqr(x - pos.X) + MathEx.Sqr(y - pos.Y));

            return d <= MdgPhysicsParams.BallRadius;
        }



        public bool IsPrimary
        {
            get;
            set;
        }

        public MdgResource(ScreenPhysicsWorld world, MdgType type, Vector2 pos, float ori)
        {
            this.physicsWorld = world;

            this.body = new ScreenRigidBody();
            this.body.Orientation = ori;
            this.body.Position = pos;
            this.body.Radius = MdgPhysicsParams.BallRadius;
            this.body.Mass = MdgPhysicsParams.BallMass;
            this.body.Elasity = MdgPhysicsParams.BallElasity;
            this.body.Friction = MdgPhysicsParams.BallFriction;
            this.body.AngularDamp = MdgPhysicsParams.BallAngularDamp;
            this.body.LinearDamp = MdgPhysicsParams.BallLinearDamp;
            this.body.Tag = this;

            world.Add(body);

            this.type = type;
            this.image = LoadImage(type, 7);
        }
        public void NotifyRemoved()
        {
            physicsWorld.Remove(body);
        }
        public override void Render(Sprite sprite)
        {
            if (image != null)
            {
                Vector2 pos = body.Position;
                float r = body.Radius;

                sprite.SetTransform(
                    Matrix.Scaling(2 * r / image.Width, 2 * r / image.Height, 1) *
                    Matrix.Translation(-r, -r, 0) * Matrix.RotationZ(-body.Orientation) * Matrix.Translation(pos.X, pos.Y, 0));

                if (IsPrimary)
                {
                    sprite.Draw(image, 0, 0, ColorValue.White);
                }
                else
                {
                    ColorValue opa = new ColorValue(MdgPhysicsParams.InactiveAlpha, 1, 1, 1);
                    sprite.Draw(image, 0, 0, opa);
                }
            }

        }

        //public override void Update(GameTime time)
        //{

        //}
        #region IMdgSelection 成员

        public MdgIconType IconType
        {
            get { return MdgIconType.Ball; }
        }

        public Vector2 Position
        {
            get { return body.Position; }
            set { body.Position = value; }
        }
        public Vector2 Velocity
        {
            get { return body.Velocity; }
            set { body.Velocity = value; }
        }
        #endregion
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

        MdgResource[] primaryBall;
        MdgPiece[][] primaryPiece;

        public MdgResourceManager()
        {
            pieces = new FastList<MdgPiece>[(int)MdgType.Count][];
            primaryBall = new MdgResource[(int)MdgType.Count];
            primaryPiece = new MdgPiece[(int)MdgType.Count][];

            balls = new FastList<MdgResource>[(int)MdgType.Count];
            
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i] = new FastList<MdgPiece>[8];
                primaryPiece[i] = new MdgPiece[8];

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
            if (pieces[(int)piece.Type][piece.BitMask].Count == 0)
            {
                primaryPiece[(int)piece.Type][piece.BitMask] = piece;
            }
            pieces[(int)piece.Type][piece.BitMask].Add(piece);
        }
        public void Add(MdgResource res)
        {
            if (balls[(int)res.Type].Count == 0)
            {
                primaryBall[(int)res.Type] = res;
            }
            balls[(int)res.Type].Add(res);
        }
        public void Remove(MdgPiece piece)
        {
            pieces[(int)piece.Type][piece.BitMask].Remove(piece);
            piece.NotifyRemoved();
            for (int i = 0; i < primaryPiece.Length; i++) 
            {
                for (int j = 0; j < primaryPiece[i].Length; j++)
                {
                    if (object.ReferenceEquals(primaryPiece[i][j], piece))
                    {
                        primaryPiece[i][j] = null;
                        i = primaryPiece.Length;
                        break;
                    }
                }
            }
        }
        public void Remove(MdgResource res)
        {
            balls[(int)res.Type].Remove(res);
            res.NotifyRemoved();
            for (int i = 0; i < primaryBall.Length; i++)
            {
                if (object.ReferenceEquals(primaryBall[i], res))
                {
                    primaryBall[i] = null;
                    i = primaryBall.Length;
                    break;
                }

            }
        }

        public MdgResource GetPrimaryResource(MdgType type)
        {
            return primaryBall[(int)type];
        }
        public MdgPiece GetPrimaryPiece(MdgType type, int bitmask)
        {
            return primaryPiece[(int)type][bitmask];
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


        public void Update(GameTime time)
        {

        }



    }

}

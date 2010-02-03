using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.GUI;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
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
        Partnership = 7 << 28
    }

    class MdgPiece : UIComponent
    {
        ScreenRigidBody body;

        /// <summary>
        ///  either 4,2 or 1
        /// </summary>
        int bitMask;

        MdgType type;

        Texture image;

        public MdgPiece(MdgType type, int piece)
        {
            
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

    class MdgResource : UIComponent
    {
        ScreenRigidBody body;

        Texture image;

        MdgType tpye;

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
}

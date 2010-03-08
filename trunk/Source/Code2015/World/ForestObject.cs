using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    class ForestObject : SceneObject, ISelectableObject
    {
        //struct Tree 
        //{
        //    public Matrix Transformation;
        //    public Model Model;
        //}

        Forest forest;
        TreeBatchModel model;

        public ForestObject(RenderSystem rs, Forest forest)
            : base(false)
        {
            this.forest = forest;


            ForestInfo info;
            info.Latitude = forest.Latitude;
            info.Longitude = forest.Longitude;
            info.Radius = forest.Radius;

            info.Amount = forest.CurrentAmount;
            info.Category = forest.Category;
            info.Type = forest.PlantKind;

            info.BigPlants = TreeModelLibrary.Instance.Get(PlantCategory.Forest, info.Type);
            info.SmallPlants = TreeModelLibrary.Instance.Get(PlantCategory.Bush, info.Type);
            model = new TreeBatchModel(rs, info);

        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {
            throw new NotImplementedException();
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        #region ISelectableObject 成员

        bool ISelectableObject.IsSelected
        {
            get;
            set;
        }

        #endregion
    }
}

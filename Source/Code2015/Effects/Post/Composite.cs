﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Vfs;

namespace Code2015.Effects
{
    class Composite : PostEffect
    {
        public Composite(RenderSystem rs)
            : base(rs)
        {
            string filePath = "composite.ps";
            FileLocation fl = FileSystem.Instance.Locate(filePath, FileLocateRule.Effects);

            LoadPixelShader(rs, fl);
        }
    }
}

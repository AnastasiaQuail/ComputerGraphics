﻿using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;

namespace GameFramework
{
    public class SurfaceComponent:GameComponent
    {

        Stopwatch clock;
        public float width, heigh;
        public float z;
        Vector4 color;
        public Vector3 startPos;

        public SurfaceComponent(Game gameObj,float width, float heigh, Vector4 color)
        {
            this.game = gameObj;
            transform = new Transform();
            clock = game.clock;
            this.width = width/2f;
            this.heigh = heigh/2f;
            this.color = color;
            z = 0;
            nameOfShader = "Shaders/BCTexture.fx";
            Initialize(game, "Shaders/BCTexture.fx",false);
        }

        public override Points[] AIStage()
        {
            return new Points[]
            {
                new Points( new Vector4(-width, -heigh, z, 1.0f), new Vector4(0,1,0,0)),
                new Points(new Vector4(-width,  heigh, z, 1.0f), new Vector4(0,0,0,0)),
                new Points(new Vector4( width,  heigh, z, 1.0f), new Vector4(1,0,0,0)),
                new Points(new Vector4(-width, -heigh, z, 1.0f), new Vector4(0,1,0,0)),
                new Points(new Vector4( width,  heigh, z, 1.0f), new Vector4(1,0,0,0)),
                new Points(new Vector4( width, -heigh, z, 1.0f), new Vector4(1,1,0,0))
            };
        }
        public override void Draw()
        {
			base.Draw();
            UpdateContext(PrimitiveTopology.TriangleList,Utilities.SizeOf<Vector4>()*2);
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(6, 0);
        }
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(game.device,
                ShaderSignature.GetInputSignature(vertexShaderBC),
                new[]
                {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("TEXCOORD",0,Format.R32G32B32A32_Float,16,0),
                });
            vbSize = Utilities.SizeOf<Vector4>() * 2;
        }


    }

}

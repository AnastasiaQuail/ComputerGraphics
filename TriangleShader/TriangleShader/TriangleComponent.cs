﻿using SharpDX;
using SharpDX.Direct3D;
using Vector4 = SharpDX.Vector4;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace GameFramework
{
    public class TriangleComponent : GameComponent
    {
        public TriangleComponent(Game gameObj) {

            this.game = gameObj;
            Initialize(game, "Shaders/BCTriangle.fx",false);
        }
         public override void Draw()
        {
            UpdateContext(PrimitiveTopology.TriangleList,Utilities.SizeOf<Vector4>()*2);
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(3, 0);

        }
        
        public new Vector4[] AIStage()
        {
            return new[]{
           new Vector4(0.0f,5f,5f,1.0f),new Vector4(1.0f,0.0f,0.0f,1f),
           new Vector4(-5f,-5f,5f,1.0f),new Vector4(0.0f,0.0f,1.0f,1.0f),
           new Vector4(5f,-5f,5f,1.0f),new Vector4(0.0f,1.0f,0.0f,1.0f)
            };
        }
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(device,
               ShaderSignature.GetInputSignature(vertexShaderBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
               });
        }

    }
}

using System;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Vector4 = SharpDX.Vector4;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using SharpDX.D3DCompiler;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameFramework
{
    class ShadowVolumeCube: StreamComponent
    {
        // private IndexBuffer indexBuffer;
        private BufferDescription indexBufferDesc;
        public Buffer indexBuffer;
        private int[] indexes;
        private Points[] vertexes;

        public ShadowVolumeCube(Game game):base(game)
        {
            CreateIndexBuffer();
        }
        private void CreateIndexBuffer()
        {
            indexBufferDesc = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            };
            AIStage();
            indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indexes); ;
        }
        public override Points[] AIStage()
        {
            indexes = new[]
            {
                0,6,2,3,1,5,
                1,0,2,7,3,7,
                4,0,6,7,5,0,
                5,4,6,2,7,1,
                0,4,5,7,1,2,
                0,1,5,7,4,6,
                2,3,7,4,6,0,
                2,1,3,1,7,6,
                0,5,4,5,6,2,
                6,7,2,1,0,4,
                1,5,7,2,3,2,
                1,0,5,4,7,3
            };
            vertexes = new Points[]

          {
               new Points(new Vector4(-0.5f,-0.5f,-0.5f,1f),new Vector4(0.0f,0.0f,0.0f,1f)),
                new Points(new Vector4(-0.5f,-0.5f,0.5f,1f),new Vector4(0.0f,0.0f,1.0f,1f)),
                new Points(new Vector4(-0.5f,0.5f,-0.5f,1f),new Vector4(0.0f,1.0f,0.0f,1f)),
                new Points(new Vector4(-0.5f,0.5f,0.5f,1f),new Vector4(0.0f,1.0f,1.0f,1f)),
                new Points(new Vector4(0.5f,-0.5f,-0.5f,1f),new Vector4(1.0f,0.0f,1.0f,1f)),
                new Points(new Vector4(0.5f,-0.5f,0.5f,1f),new Vector4(1.0f,0.0f,1.0f,1f)),
                new Points(new Vector4(0.5f,0.5f,-0.5f,1f),new Vector4(1.0f,1.0f,0.0f,1f)),
                new Points(new Vector4(0.5f,0.5f,0.5f,1f),new Vector4(1.0f,1.0f,1.0f,1f))
            };

            return vertexes;
        }
        public override void Update()
        {
            //Output for stream output buffer
            context.StreamOutput.SetTarget(steamOutputBuf, 0);
            context.InputAssembler.InputLayout = streamLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            context.VertexShader.Set(vertexStream);
            context.PixelShader.Set(null);
            context.GeometryShader.Set(geometryShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.SetConstantBuffer(1, game.lightBuffer);
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            context.DrawIndexed(indexes.Length, 0, 0);
            context.StreamOutput.SetTargets(null);
            context.GeometryShader.Set(null);
        }
        public override void Draw()
        {
            // -------------Start drowing from stream ----------------//
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleListWithAdjacency;
            context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();
            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);
            SetConstantData();
            ResterizeStage();
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            context.DrawIndexed(indexes.Length, 0, 0);
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

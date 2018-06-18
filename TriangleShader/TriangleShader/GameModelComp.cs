using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using GameFramework;
using System;

namespace GameFramework
{
    public class GameModelComp : GameComponent
    {
        ObjLoader objLoader;
        string fileName;
        public VertexBufferBinding BufferBinding;
        public GameComponent parent;

        public GameModelComp(Game game, string filename)
        {
            this.game = game;
            fileName = filename;
            objLoader = new ObjLoader();
            transform = new Transform();
            nameOfShader = "Shaders/BCAmbient.fx";
            Initialize(game, "Shaders/BCAmbient.fx", true);
            parent = null;
            constantData = new ConstantData();

            transform = new Transform();
            WorldViewProjMatrix = Matrix.Identity;
        }
        public override void CreateVertexShader()
        {
            //Create VertexBuffer  
            objLoader.LoadObjModel(fileName, out vertexBuffer, out verticesCount, game);
            BufferBinding = new VertexBufferBinding(vertexBuffer, 48, 0);
        }
        public override void Draw()
        {
            base.Draw();
            UpdateContext(PrimitiveTopology.TriangleList, Utilities.SizeOf<Vector4>() * 3);
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.InputAssembler.SetVertexBuffers(0, BufferBinding);
            ResterizeStage();
            game.context.Draw(verticesCount, 0);
        }
        public override void ShadowDraw()
        {
            base.ShadowDraw();
            // UpdateShadow();
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
            game.context.InputAssembler.SetVertexBuffers(0, BufferBinding);
            ResterizeStage();
            game.context.Draw(verticesCount, 0);

        }

        public override void Update()
        {
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();

            if (parent != null)
            {
                WorldMatrix = WorldMatrix * parent.transform.GetWorldMatrix();
            }
            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);

            SetConstantData();
            if (lightFlag)
            {
                game.context.UpdateSubresource(ref game.sceneLight.light, game.lightBuffer);
            }
        }
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(game.device,
               ShaderSignature.GetInputSignature(vertexShaderBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("NORMAL",0,Format.R32G32B32A32_Float,16,0),
                    new InputElement("TEXCOORD",0,Format.R32G32B32A32_Float,32,0),
               });
            vbSize = Utilities.SizeOf<Vector4>() * 3;
        }
    }
}

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;

namespace GameFramework
{
        public struct ConstantDataDef
        {
            public Matrix WorldViewProj;
            public Matrix World;
            public Matrix InvertWorld;
            public Vector4 ViewPos;
            public Matrix InverseProjectionView;
    }

    class SurfacePlane :GameComponent
    {
        private CompilationResult pixelLightShaderBC;
        private CompilationResult vertexLightShaderBC;
        public VertexShader vertexShader2;
        public PixelShader pixelShader2;
        public ConstantDataDef data;
        private int[] vertexId;
        public VertexBufferBinding vertexBufBinding;

        public SurfacePlane(Game gameObj, float width, float heigh, Vector4 color)
        {
           
            textureFile = "Texture.jpg";
            data = new ConstantDataDef();
            vertexId = new int[]
            {
                0, 1, 2, 3
            };
            this.game = gameObj;
            transform = new Transform();
            device = game.device;
            context = game.context;
            lightFlag = false;
            textureLoader = new TextureLoader(game);

            createLightVertexShader("Shaders/BCDefferedLight.fx");
            createLightPixelShader("Shaders/BCDefferedLight.fx");
            createVertexShader("Shaders/BCDefferedLight.fx");
            createPixelShader("Shaders/BCDefferedLight.fx");

            CreateLayout();

            CreateVertexShader();

            CreateConstantBuffer();

            if (lightFlag)
            {
                CreateLightPixelShader();
                CreateLightVertexShader();
            }

            CreateRasterizwState();
            vertexBufBinding = new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<uint>(), 0);
        }

        public void createLightVertexShader(string filename)
        {
            try
            {
                vertexLightShaderBC = ShaderBytecode.CompileFromFile(filename, "LightingVS", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            vertexShader2 = new VertexShader(device, vertexLightShaderBC);
        }

        public void createLightPixelShader(string filename)
        {
            pixelLightShaderBC = ShaderBytecode.CompileFromFile(filename, "LightingPS", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShader2 = new PixelShader(device, pixelLightShaderBC);
        }
        public void createVertexShader(string filename)
        {
            try
            {
                vertexShaderBC = ShaderBytecode.CompileFromFile(filename, "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            vertexShader = new VertexShader(device, vertexShaderBC);
        }

        public void createPixelShader(string filename)
        {
            pixelShaderBC = ShaderBytecode.CompileFromFile(filename, "PSMain", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShader = new PixelShader(device, pixelShaderBC);
        }

        public override void Update()
        {
            base.Update();
            game.context.UpdateSubresource(ref game.sceneLight.light, game.lightBuffer);
        }
        public override void SetConstantData()
        {
            //Set data
            InvertWorld = WorldMatrix;
            InvertWorld.Invert();

            data.World = WorldMatrix;
            data.InvertWorld = InvertWorld;
            data.WorldViewProj = WorldViewProjMatrix;
            data.ViewPos = new Vector4(transform.Position, 1);
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            data.InverseProjectionView = Matrix.Invert(ViewProjMatrix);

            context.UpdateSubresource(ref data, constantBuffer);
        }
        public override void CreateConstantBuffer()
        {
            //Create Constant buffer
            constantBuffer = new Buffer(device, (Utilities.SizeOf<ConstantDataDef>()),
               ResourceUsage.Default,
               BindFlags.ConstantBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None, 0);
        }
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(game.device,
                ShaderSignature.GetInputSignature(vertexShaderBC),
                new[]
                {
                    new InputElement("SV_VertexID",0, SharpDX.DXGI.Format.R8_UInt,0,0),
                });
            vbSize = Utilities.SizeOf<uint>();
        }
        public override void CreateVertexShader()
        {
            //Vertex Shader
            bufferDesc = new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            };
            vertexBuffer = Buffer.Create(device, vertexId, bufferDesc);
            verticesCount = vertexId.Length;
        }
        public override void Draw()
        {
            base.Draw();
            UpdateContext(PrimitiveTopology.TriangleList, Utilities.SizeOf<uint>());
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.InputAssembler.SetVertexBuffers(0, vertexBufBinding);
            ResterizeStage();
            game.context.Draw(verticesCount, 0);
        }
    }
}

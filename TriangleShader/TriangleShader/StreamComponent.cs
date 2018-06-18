using SharpDX;
using Vector4 = SharpDX.Vector4;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace GameFramework
{
    class StreamComponent:GameComponent
    {
        public GeometryShader geometryShader;
        public Buffer steamOutputBuf;
        public CompilationResult vertexStreamBC;
        public VertexShader vertexStream;
        public CompilationResult streamBC;
        public InputLayout streamLayout;
        public VertexBufferBinding streamOutputBufferBinding;
        public VertexBufferBinding vertexBufferBinding;

        public StreamComponent(Game game)
        {
            transform = new Transform();
            device = game.device;
            CreateSteamShaders("Shaders/GeometryShader.fx");
            CreateGeometryShader("Shaders/GeometryShader.fx");
            CreateStreamOutputBuffer();
            CreateStreamLayout();
            Initialize(game, "Shaders/BCTriangle.fx", false);

            streamOutputBufferBinding = new VertexBufferBinding(steamOutputBuf, Utilities.SizeOf<Vector4>() * 2, 0);
            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>() * 2, 0);


            // For Stream output stage

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
            context.Draw(3, 0);
            context.StreamOutput.SetTargets(null);
            context.GeometryShader.Set(null);
        }

        public override void Draw()
        {
            
        }

        public void DrawFromGB()
        {
            // -------------Start drowing from stream ----------------//
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            context.InputAssembler.SetVertexBuffers(0, streamOutputBufferBinding);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();
            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);
            SetConstantData();
            ResterizeStage();
            context.DrawAuto();
        }
        
        public void CreateGeometryShader(string filename)
        {
            streamBC = ShaderBytecode.CompileFromFile("Shaders/GeometryShader.fx", "GSStream", "gs_5_0");
            StreamOutputElement elementPos = new StreamOutputElement(0, "POSITION", 0, 0, 4, 0);
            StreamOutputElement elementCol = new StreamOutputElement(0, "COLOR", 0, 0, 4, 0);
            StreamOutputElement[] arrayElem = { elementPos, elementCol };
            int[] sizes = { Utilities.SizeOf<Vector4>()*2 };
            geometryShader = new GeometryShader(
                this.device,
                streamBC,
                arrayElem,
                sizes,
                -1,
                null);
        }
        public void CreateStreamOutputBuffer()
        {
            int maxOutputPoints = 1000;
            steamOutputBuf = new Buffer(device, new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer | BindFlags.StreamOutput,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                StructureByteStride = Utilities.SizeOf<Vector4>() * 2,
                SizeInBytes = Utilities.SizeOf<Vector4>() * 2 * maxOutputPoints
            });
        }
        public void CreateSteamShaders(string filename)
        {
            vertexStreamBC = ShaderBytecode.CompileFromFile("Shaders/GeometryShader.fx", "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            vertexStream= new VertexShader(device, vertexStreamBC);
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
            vertexBuffer = Buffer.Create(device,  AIStage(), bufferDesc);
        }
        public  void CreateStreamLayout()
        {
            //Create layout
            streamLayout = new InputLayout(device,
               ShaderSignature.GetInputSignature(vertexStreamBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
               });
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

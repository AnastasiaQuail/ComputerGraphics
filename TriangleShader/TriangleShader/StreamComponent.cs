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
        private Buffer steamOutputBuf;
        private CompilationResult vertexStreamBC;
        private VertexShader vertexStream;
        private CompilationResult streamBC;
        private InputLayout streamLayout;
        private VertexBufferBinding streamOutputBufferBinding;
        private VertexBufferBinding vertexBufferBinding;

        public StreamComponent(Game game)
        {
            this.game = game;
            transform = new Transform();
            device = game.device;
            //CreateSteamShaders("Shaders/GeometryShader.fx");
            //CreateGeometryShader("Shaders/GeometryShader.fx");
            //CreateStreamOutputBuffer();
            //CreateStreamLayout();
            Initialize(game, "Shaders/BCTriangle.fx", false);

            //streamOutputBufferBinding = new VertexBufferBinding(steamOutputBuf, Utilities.SizeOf<Vector4>() * 2, 0);
            //vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>() * 2, 0);

            // For Stream output stage

        }
        public override void Update()
        {
            
        }

        public override void Draw()
        {
            //Output for stream output buffer
            //context.StreamOutput.SetTarget(steamOutputBuf, 0);
            //context.InputAssembler.InputLayout = streamLayout;
            //context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            //context.InputAssembler.SetVertexBuffers(0,vertexBufferBinding);
            //context.VertexShader.Set(vertexStream);
            //context.PixelShader.Set(null);
            //context.GeometryShader.Set(geometryShader);
            //context.DrawAuto();
            //context.StreamOutput.SetTargets(null);
            //context.GeometryShader.Set(null);
            // -------------Start drowing from stream ----------------//
            //context.InputAssembler.InputLayout = layout;
            //context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            //context.VertexShader.Set(vertexShader);
            //context.PixelShader.Set(pixelShader);
            //context.VertexShader.SetConstantBuffer(0, constantBuffer);
            //game.mycamera.GetVPMatrix(out ViewProjMatrix);
            //WorldMatrix = transform.GetWorldMatrix();
            //Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);
            //  SetConstantData();
            UpdateContext(PrimitiveTopology.LineList, Utilities.SizeOf<Vector4>() * 2);

            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
            
            ResterizeStage();
            context.Draw(3,0);
        }
        
        //private void CreateGeometryShader(string filename)
        //{
        //    streamBC = ShaderBytecode.CompileFromFile("Shaders/GeometryShader.fx", "GSStream", "gs_5_0");
        //    StreamOutputElement elementPos = new StreamOutputElement(0, "POSITION", 0, 0, 1, 0);
        //    StreamOutputElement elementCol = new StreamOutputElement(0, "COLOR", 0, 0, 1, 0);
        //    StreamOutputElement[] arrayElem = { elementPos, elementCol };
        //    int[] sizes = { Utilities.SizeOf<Vector4>(), Utilities.SizeOf<Vector4>() };
        //    geometryShader = new GeometryShader(
        //        this.device,
        //        streamBC,
        //        arrayElem,
        //        sizes,
        //        -1,
        //        null);
        //}
        //private void CreateStreamOutputBuffer()
        //{
        //    int maxOutputPoints = 1000;
        //    steamOutputBuf = new Buffer(device, new BufferDescription
        //    {
        //        BindFlags = BindFlags.VertexBuffer | BindFlags.StreamOutput,
        //        CpuAccessFlags = CpuAccessFlags.None,
        //        OptionFlags = ResourceOptionFlags.None,
        //        Usage = ResourceUsage.Default,
        //        StructureByteStride = Utilities.SizeOf<Vector4>() * 2,
        //        SizeInBytes = Utilities.SizeOf<Vector4>() * 2 * maxOutputPoints
        //    });
        //}
        //private void CreateSteamShaders(string filename)
        //{
        //    vertexStreamBC = ShaderBytecode.CompileFromFile("Shaders/GeometryShader.fx", "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
        //    vertexStream= new VertexShader(device, vertexStreamBC);
        //}
        public override Points[] AIStage()
        {

            return new Points[]{
           new Points(new Vector4(0.0f,5f,5f,1.0f),new Vector4(1.0f,0.0f,0.0f,1f)),
           new Points(new Vector4(-5f,-5f,5f,1.0f),new Vector4(0.0f,0.0f,1.0f,1.0f)),
           new Points(new Vector4(5f,-5f,5f,1.0f),new Vector4(0.0f,1.0f,0.0f,1.0f))
            };
        }
        //public  void CreateStreamLayout()
        //{
        //    //Create layout
        //    streamLayout = new InputLayout(device,
        //       ShaderSignature.GetInputSignature(vertexStreamBC),
        //       new[]
        //       {
        //            new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
        //            new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0),
        //       });
        //}
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(device,
               ShaderSignature.GetInputSignature(vertexShaderBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
               });
        }


    }
}

using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Vector4 = SharpDX.Vector4;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using System.IO;
using System;

namespace GameFramework
{
    public class GameComponent
    {
        public Game game;
        public InputLayout layout;
        public VertexShader vertexShader;
        public PixelShader pixelShader;
        public CompilationResult vertexShaderBC, pixelShaderBC;
        public Buffer vertexBuffer;
        public BufferDescription bufferDesc;
        public Matrix ViewProjMatrix;
        public Matrix WorldViewProjMatrix, WorldMatrix,InvertWorld;
        public Buffer constantBuffer,lightBuffer;
        ShaderResourceView textureView;
        Texture2D texture;
        SamplerState sampler;
        public Transform transform;
        public string textureFile;
        private TextureLoader textureLoader;
        public ConstantData constantData;
		private LightData light;
        bool lightFlag;

        public struct Points
        {
            private Vector4 vector4;
            private Vector4 vector2;

            public Points(Vector4 vector4, Vector4 vector2) : this()
            {
                this.vector4 = vector4;
                this.vector2 = vector2;
            }
        }



        public void Initialize(Game game, string nameOfFile, bool IsLight)
        {
            this.game = game;
            lightFlag = IsLight;
            textureLoader = new TextureLoader(game);
            transform = new Transform();

            CreateVertexBuffer(nameOfFile);

            CreatePixelBuffer(nameOfFile);

            CreateLayout();

            CreateVertexShader();

            CreateConstantBuffer();
            

        }
        public virtual void Update()
        {
            //get transform from WorldViewProjMatrix
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();
            
            Matrix.Multiply(ref  WorldMatrix,ref ViewProjMatrix, out WorldViewProjMatrix);

			//Set data
			InvertWorld = WorldMatrix;
			InvertWorld.Invert();

			constantData.World = WorldMatrix;
			constantData.InvertWorld = InvertWorld;
			constantData.WorldViewProj = WorldViewProjMatrix;
            constantData.ViewPos =new Vector4(transform.Position,1);

            game.context.UpdateSubresource(ref constantData, constantBuffer);
			

			transform.SetFrameTime(game.clock.ElapsedMilliseconds);

        }
        public virtual void Draw() {
            if(lightFlag)
			    game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
		}
        public void Dispose()
        {
            //Dispose all resourses
            vertexShader.Dispose();
            vertexShaderBC.Dispose();
            pixelShader.Dispose();
            pixelShaderBC.Dispose();
            layout.Dispose();
            vertexBuffer.Dispose();

        }
        public virtual Points[] AIStage()
        {
            return new Points[0];
        }
       


        public virtual void CreateVertexBuffer(string filename)
        {
            //Create VertexBuffer  
            try
            {
                vertexShaderBC = ShaderBytecode.CompileFromFile(filename, "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }catch(ArgumentNullException e) { Console.WriteLine(e.Message); }
            vertexShader = new VertexShader(game.device, vertexShaderBC);
        }
        private void CreatePixelBuffer(string filename)
        {
            //and PixelBuffer
            pixelShaderBC = ShaderBytecode.CompileFromFile(filename, "PSMain", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShader = new PixelShader(game.device, pixelShaderBC);
        }
        public virtual void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(game.device,
               ShaderSignature.GetInputSignature(vertexShaderBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("TEXCOORD",0,Format.R32G32B32A32_Float,16,0),
               });
        }
        public virtual void CreateConstantBuffer()
        {
            //Create Constant buffer
            constantBuffer = new Buffer(game.device, (Utilities.SizeOf<ConstantData>()),
               ResourceUsage.Default,
               BindFlags.ConstantBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None, 0);
        }
        
        public virtual void CreateVertexShader()
        {
            //Vertex Shader
            bufferDesc = new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            };
            vertexBuffer = Buffer.Create(game.device, AIStage(), bufferDesc);
        }

        public virtual void UpdateContext(PrimitiveTopology topology)
        {
            //Update game.context
            game.context.InputAssembler.InputLayout = layout;
            game.context.InputAssembler.PrimitiveTopology = topology;
            game.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>() * 2, 0));
            game.context.VertexShader.Set(vertexShader);
            game.context.PixelShader.SetShaderResource(0, textureView);
            game.context.PixelShader.SetSampler(0, sampler);
            game.context.PixelShader.Set(pixelShader);
			game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
			game.context.PixelShader.SetConstantBuffer(0, constantBuffer);
			game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);



		}
        public void ResterizeStage()
        {
            //Resterizer Stage
            game.context.Rasterizer.State = new RasterizerState(game.device, new RasterizerStateDescription
            {
                CullMode = CullMode.Back,
                FillMode = FillMode.Solid 
            });
            game.context.OutputMerger.SetTargets(game.depthView, game.renderView);
            game.context.Rasterizer.SetViewport(0, 0, game.Form.ClientSize.Width, game.Form.ClientSize.Height, 0.0f, 1.0f);
            
        }

        public void SetWorldMatrix(Matrix newMatrix)
        {
            WorldMatrix = newMatrix;
        }

       public void CreateTextureShader()
        {
           
            if (File.Exists(textureFile))
            {
                texture = textureLoader.LoadTextureFromFile(textureFile);
                textureView = new ShaderResourceView(game.device, texture);

                game.context.GenerateMips(textureView);
            }
        }
        public void CreateTextureSampler()
        {
            sampler = new SamplerState(game.device, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Always,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            });
        }

        public void SetTextureFile(string fileName)
        {
            textureFile = fileName;
            CreateTextureShader();
            CreateTextureSampler();

        }

    }
}

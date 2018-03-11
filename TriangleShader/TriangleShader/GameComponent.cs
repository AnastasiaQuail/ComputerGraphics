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
        public Matrix WorldViewProjMatrix, WorldMatrix,InvertWorld, InvertVP;
        public Buffer constantBuffer,lightBuffer;
        public ShaderResourceView textureView;
        Texture2D texture;
        SamplerState sampler;
        public Transform transform;
        public string textureFile;
        private TextureLoader textureLoader;
        public ConstantData constantData;
        public bool lightFlag;
        public SharpDX.Direct3D11.Device device;
        public DeviceContext context;
        public string nameOfShader;
        private BufferDescription shadowBufferDesc;
        public Buffer shadowVertexBuffer;
        public CompilationResult shadowVertexShaderBC;
        public VertexShader shadowVertexShader;
        public int verticesCount;

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



        public virtual void Initialize(Game game, string nameOfFile, bool IsLight)
        {
            this.game = game;
            nameOfShader = nameOfFile;
            device = game.device;
            context = game.context;
            transform = new Transform();
            lightFlag = IsLight;
            textureLoader = new TextureLoader(game);

            CreateVertexBuffer(nameOfFile);

            CreatePixelBuffer(nameOfFile);

            CreateLayout();

            CreateVertexShader();

            CreateConstantBuffer();

            if (lightFlag)
            {
                CreateShadowVertexBuffer();
            }


        }

        private void CreateShadowVertexBuffer()
        {
            try
            {
                shadowVertexShaderBC = ShaderBytecode.CompileFromFile("Shaders/BCLight.fx", "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            shadowVertexShader = new VertexShader(device, shadowVertexShaderBC); 
        }

        public virtual void Update()
        {
            //get transform from WorldViewProjMatrix
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();

            Matrix.Multiply(ref  WorldMatrix,ref ViewProjMatrix, out WorldViewProjMatrix);

            float cuurTime = game.clock.ElapsedMilliseconds;
            // invert viweproj
            SetConstantData();
            float nextTime = game.clock.ElapsedMilliseconds;

            transform.SetFrameTime(nextTime-cuurTime);
        }
        public virtual void Draw() {
            
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
            vertexShader = new VertexShader(device, vertexShaderBC);
        }
        private void CreatePixelBuffer(string filename)
        {
            //and PixelBuffer
            pixelShaderBC = ShaderBytecode.CompileFromFile(filename, "PSMain", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShader = new PixelShader(device, pixelShaderBC);
        }
        public virtual void CreateLayout()
        {
            //Create layout
                layout = new InputLayout(device,
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
            constantBuffer = new Buffer(device, (Utilities.SizeOf<ConstantData>()),
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
            vertexBuffer = Buffer.Create(device, AIStage(), bufferDesc);
            verticesCount = AIStage().Length;
        }

        public virtual void UpdateContext(PrimitiveTopology topology, int vbSize )
        {
            //Update context
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = topology;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vbSize, 0));
            context.VertexShader.Set(vertexShader);
            context.PixelShader.SetShaderResource(0, textureView);
            context.PixelShader.SetSampler(0, sampler);
            if (lightFlag)
            {
                game.context.PixelShader.SetShaderResource(1, game.shadowResourceView);
                game.context.PixelShader.SetSampler(1, game.shadowSampler);
            }
            context.PixelShader.Set(pixelShader);
			context.VertexShader.SetConstantBuffer(0, constantBuffer);
			context.PixelShader.SetConstantBuffer(0, constantBuffer);
            if(lightFlag)
            { 
                context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
                
            }
        }

        public virtual void ShadowDraw()
        {
            UpdateShadow();

            //get transform from WorldViewProjMatrix
            game.sceneLight.camera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();
            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);

            SetConstantData();
        }

        public void UpdateShadow()
        {
            //Update context
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>()*3, 0));
            context.VertexShader.Set(shadowVertexShader);
            context.PixelShader.Set(null);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
        }

        public virtual void ResterizeStage()
        {
            //Resterizer Stage
            context.Rasterizer.State = new RasterizerState(device, new RasterizerStateDescription
            {
                CullMode = CullMode.Back,
                FillMode = FillMode.Solid,
                DepthBias = 10,
                SlopeScaledDepthBias = 0.0001f
            });
            
        }

        public void SetWorldMatrix(Matrix newMatrix)
        {
            WorldMatrix = newMatrix;
        }

       public virtual void CreateTextureShader()
        {

            if (File.Exists(textureFile))
            {
                texture = textureLoader.LoadTextureFromFile(textureFile);
                textureView = new ShaderResourceView(device, texture);
                
            }
            context.GenerateMips(textureView);
        }
        public virtual void CreateTextureSampler()
        {
            sampler = new SamplerState(device, new SamplerStateDescription()
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
       
        public virtual void SetTextureFile(string fileName)
        {
            textureFile = fileName;
            CreateTextureShader();
            CreateTextureSampler();

        }
        public void SetConstantData()
        {
            //Set data
            InvertWorld = WorldMatrix;
            InvertWorld.Invert();

            InvertVP = WorldViewProjMatrix;
            InvertVP.Invert();

            constantData.World = WorldMatrix;
            constantData.InvertWorld = InvertWorld;
            constantData.WorldViewProj = WorldViewProjMatrix;
            constantData.ViewPos = new Vector4(transform.Position, 1);
            constantData.InverseProjectionView = InvertVP;

            context.UpdateSubresource(ref constantData, constantBuffer);
        }
    }
}

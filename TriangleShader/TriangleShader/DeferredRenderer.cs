using System;
using System.Collections.Generic;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Vector4 = SharpDX.Vector4;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;

namespace GameFramework
{
    class DeferredRenderer
    {
        Game game;
        List<LightCamera> lightList;
        List<GameComponent> components;
        private Texture2DDescription textureDescription;
        private Texture2D texture;
        Texture2D diffuseBuf, positionBuf, normalBuf;
        private ShaderResourceView positionResourceView;
        private RenderTargetView posRenderTarget;
        private ShaderResourceView normalResourceView;
        private RenderTargetView normalRenderTarget;
        private ShaderResourceView diffuseResourceView;
        private RenderTargetView diffuseRenderTarget;
        private SamplerState sampler;
        private CompilationResult vertexShaderBC;
        private VertexShader vertexShader;
        private CompilationResult pixelShaderBC;
        private PixelShader pixelShader;
        private CompilationResult lightVertexShaderBC;
        private VertexShader lightVertexShader;
        private CompilationResult lightPixelShaderBC;
        private PixelShader lightPixelShader;

        const int BUFFER_COUNT = 3;
        private VertexBufferBinding vertexBufBinding;
        private RenderTargetView[] renderTargets;
        SurfaceComponent screen;

        public DeferredRenderer(Game gameObj)
        {
            game = gameObj;
            lightList = new List<LightCamera>();
            lightList.Add(game.sceneLight);
            GameModelComp sphere = new GameModelComp(game, "NBA BASKETBALL.obj");
            sphere.SetTextureFile("Moon.jpg");
            components = game.Components;

            screen = new SurfaceComponent(game, game.Form.Width, game.Form.Height, new Vector4(0f, 0f, 0f, 1f));
            Initialize("Shaders/BCDeffered.fx");

        }
        private void Initialize(string filename)
        {
            CreateBasicVS(filename);
            CreateBasicPS(filename);
            CreateLightVS(filename);
            CreateLightPS(filename);

            Create_GBuffer();
        }
        public void Render()
        {
            GPass();
            LightPass();
        }
        public void GPass()
        {
            game.context.ClearDepthStencilView(game.depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            game.context.ClearRenderTargetView(renderTargets[0], Color.DarkSlateBlue);
            game.context.ClearRenderTargetView(renderTargets[1], Color.Black);
            game.context.ClearRenderTargetView(renderTargets[2], Color.Black);

            game.context.OutputMerger.SetRenderTargets(game.depthView,renderTargets);

            foreach (GameComponent component in components)
            {
                if (component.lightFlag)
                {
                    vertexBufBinding = new VertexBufferBinding(component.vertexBuffer, Utilities.SizeOf<Vector4>() * 3, 0);
                    component.Update();

                    game.context.InputAssembler.InputLayout = component.layout;
                    game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    game.context.InputAssembler.SetVertexBuffers(0, vertexBufBinding);
                    game.context.VertexShader.Set(vertexShader);
                    game.context.PixelShader.Set(pixelShader);
                    game.context.PixelShader.SetShaderResource(0, component.textureView);
                    game.context.PixelShader.SetSampler(0, sampler);
                    game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
                    game.context.VertexShader.SetConstantBuffer(0, component.constantBuffer);

                    component.ResterizeStage();
                    game.context.Draw(component.verticesCount, 0);
                }
            }

        }
        public void LightPass()
        {
           //FrontBackRender(lightList[0]);
            
                    vertexBufBinding = new VertexBufferBinding(screen.vertexBuffer, Utilities.SizeOf<Vector4>() * 3, 0);
                    screen.Update();

                    game.context.InputAssembler.InputLayout = screen.layout;
                    game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    game.context.InputAssembler.SetVertexBuffers(0, vertexBufBinding);
                    game.context.VertexShader.Set(vertexShader);
                    game.context.PixelShader.Set(pixelShader);

                    game.context.PixelShader.SetShaderResource(1, diffuseResourceView);
                    game.context.VertexShader.SetShaderResource(2, normalResourceView);
                    game.context.VertexShader.SetShaderResource(3, positionResourceView);
                    game.context.PixelShader.SetSampler(0, sampler);

                    game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
                    game.context.VertexShader.SetConstantBuffer(0, screen.constantBuffer);
                    game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
                    game.context.PixelShader.SetConstantBuffer(0, screen.constantBuffer);

                    screen.ResterizeStage();
                    game.context.Draw(screen.verticesCount, 0);
        }
        private void Create_GBuffer()
        {
            CreatePositionBuff();
            CreateNormalBuf();
            CreateDiffuseBuf();

            CreateSampler();

            renderTargets = new RenderTargetView[BUFFER_COUNT];
            renderTargets[0] = diffuseRenderTarget;
            renderTargets[1] = normalRenderTarget;
            renderTargets[2] = posRenderTarget;
        }
        private void CreateBasicVS(string filename)
        {
            try
            {
                vertexShaderBC = ShaderBytecode.CompileFromFile(filename, "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            vertexShader = new VertexShader(game.device, vertexShaderBC);
        }
        private void CreateBasicPS(string filename)
        {
            pixelShaderBC = ShaderBytecode.CompileFromFile(filename, "PSMain", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShader = new PixelShader(game.device, pixelShaderBC);
        }
        private void CreateLightVS(string filename)
        {
            try
            {
                lightVertexShaderBC = ShaderBytecode.CompileFromFile(filename, "LightingVS", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            lightVertexShader = new VertexShader(game.device, lightVertexShaderBC);
        }
        private void CreateLightPS(string filename)
        {
            lightPixelShaderBC = ShaderBytecode.CompileFromFile(filename, "LightingPS", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            lightPixelShader = new PixelShader(game.device, lightPixelShaderBC);
        }
        private void CreatePositionBuff()
        { 
            positionBuf = new Texture2D(game.device, new Texture2DDescription
            {
                Height = game.Form.Height,
                Width = game.Form.Width,
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0)
            });
            positionResourceView = new ShaderResourceView(game.device, diffuseBuf, new ShaderResourceViewDescription
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });

            posRenderTarget = new RenderTargetView(game.device, positionBuf);
        }
        private void CreateDiffuseBuf()
        {
            diffuseBuf = new Texture2D(game.device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                MipLevels = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = game.Form.ClientSize.Height,
                Width = game.Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None
            });
            diffuseResourceView = new ShaderResourceView(game.device, diffuseBuf, new ShaderResourceViewDescription
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });

            diffuseRenderTarget = new RenderTargetView(game.device, diffuseBuf);
        }
        private void CreateNormalBuf()
        {
            normalBuf = new Texture2D(game.device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                MipLevels = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32G32B32A32_Float,
                Height = game.Form.ClientSize.Height,
                Width = game.Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None
            });
            normalResourceView = new ShaderResourceView(game.device, diffuseBuf, new ShaderResourceViewDescription
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });

            normalRenderTarget = new RenderTargetView(game.device, normalBuf);
        }
        public void CreateSampler()
        {
            sampler = new SamplerState(game.device, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Less,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            });
        }
    }
}

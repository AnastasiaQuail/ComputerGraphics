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
    class DeferredRenderer:Render
    {
        Game game;
        List<LightCamera> lightList;
        List<GameComponent> components;
        private SurfaceComponent screen2;
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
        SurfacePlane screen;
        private GameModelComp sphere;
        private VertexBufferBinding comVertexBufBinding;
        private GameModelComp src;

        public DeferredRenderer(Game gameObj)
        {
            game = gameObj;
            lightList = new List<LightCamera> {game.sceneLight};
            sphere = new GameModelComp(game, "earth.obj");
            Vector4 pos = lightList[0].light.Position;
            sphere.transform.Position = new Vector3(pos.X, pos.Y, pos.Z);
            sphere.SetTextureFile("Moon.jpg");
            components = game.Components;

            screen = new SurfacePlane(game, game.Form.Width, game.Form.Height, new Vector4(0f, 0f, 0f, 1f));
            screen.transform.Position += new Vector3(0f, 0f, -15f);
            screen.SetTextureFile("Moon.jpg");

            src = new GameModelComp(game, "crate.obj");
            src.transform.Position += new Vector3(0f, 0f, -15f);
            src.SetTextureFile("Moon.jpg");


            Initialize("Shaders/BCDeffered.fx");

        }
        public void Initialize(string filename)
        {
            base.Initialize(this.game);
            CreateBasicVS(filename);
            CreateBasicPS(filename);

            Create_GBuffer();
            
        }
        public override void goRender()
        {
            //GPass();

            LightPass();

            game.swapChain.Present(0, PresentFlags.None);  
        }
        public void GPass()
        {
            game.context.OutputMerger.SetRenderTargets(game.depthView, renderTargets);

            //game.context.OutputMerger.SetTargets(game.shadowView, game.renderView);
            
            game.context.ClearDepthStencilView(game.shadowView, DepthStencilClearFlags.Depth, 1.0f, 0);/////////

            game.context.ClearRenderTargetView(renderTargets[0], Color.DarkSlateBlue);
            game.context.ClearRenderTargetView(renderTargets[1], Color.Black);
            game.context.ClearRenderTargetView(renderTargets[2], Color.Black);
            //game.context.ClearRenderTargetView(game.renderView, Color.DarkSlateBlue);


            foreach (GameComponent component in components)
            {
                if (component.lightFlag)
                {
                    comVertexBufBinding = new VertexBufferBinding(component.vertexBuffer, component.vbSize, 0);
                    component.Update();

                    game.context.InputAssembler.InputLayout = component.layout;
                    game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    game.context.InputAssembler.SetVertexBuffers(0, comVertexBufBinding);
                    game.context.VertexShader.Set(vertexShader); //////
                    game.context.PixelShader.SetShaderResource(0, component.textureView);
                    game.context.PixelShader.SetSampler(0, sampler);
                    game.context.PixelShader.Set(pixelShader);   ////
                    game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
                    game.context.VertexShader.SetConstantBuffer(0, component.constantBuffer);
                    game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
                    game.context.PixelShader.SetConstantBuffer(0, component.constantBuffer);

                    component.ResterizeStage();
                    game.context.Draw(component.verticesCount, 0);
                    
                }

            }
            

        }
        public void LightPass()
        {
            //FrontBackRender(lightList[0]);
           // game.context.OutputMerger.ResetTargets();
            game.context.OutputMerger.SetTargets(game.depthView, game.renderView);
            //game.context.OutputMerger.DepthStencilState = depthStateNoZBuf;
           // game.context.OutputMerger.SetTargets(game.shadowView, game.renderView);

           // game.context.ClearRenderTargetView(game.renderView, Color.DarkSlateBlue);
            //game.context.ClearDepthStencilView(game.depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
        
            src.Update();
            game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            game.context.InputAssembler.InputLayout = null;
            game.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(null, 0, 0));
            game.context.VertexShader.Set(screen.vertexShader);
            game.context.PixelShader.Set(screen.pixelShader);
            game.context.PixelShader.SetShaderResource(1, diffuseResourceView);
            game.context.PixelShader.SetShaderResource(2, normalResourceView);
            game.context.PixelShader.SetShaderResource(3, positionResourceView);
            game.context.InputAssembler.SetIndexBuffer(null, Format.R32_UInt, 0);
            if (src.lightFlag)
            {
                game.context.PixelShader.SetShaderResource(1, game.shadowResourceView);
                game.context.PixelShader.SetSampler(1, game.shadowSampler);
            }
            game.context.VertexShader.SetConstantBuffer(0, src.constantBuffer);
            game.context.PixelShader.SetConstantBuffer(0, src.constantBuffer);
            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            src.ResterizeStage();
            game.context.Draw(4, 0);


            // game.context.Rasterizer.State = new RasterizerState(game.device, new RasterizerStateDescription() {CullMode = CullMode.None, FillMode = FillMode.Wireframe});

            //game.context.InputAssembler.InputLayout = screen.layout;
            //game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //game.context.InputAssembler.SetVertexBuffers(0, screen.vertexBufBinding);
            //game.context.VertexShader.Set(screen.vertexShader);

            //game.context.PixelShader.SetShaderResource(1, diffuseResourceView);
            //game.context.PixelShader.SetShaderResource(2, normalResourceView);
            //game.context.PixelShader.SetShaderResource(3, positionResourceView);
            //game.context.PixelShader.SetSampler(0, sampler);
            //game.context.PixelShader.Set(screen.pixelShader);

            //game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            //game.context.VertexShader.SetConstantBuffer(0, screen.constantBuffer);
            //game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            //game.context.PixelShader.SetConstantBuffer(0, screen.constantBuffer);

            // screen.ResterizeStage();
            // game.context.Draw(screen.verticesCount, 0);
            //////////////////////////////////////////////////////////////////// 
            //  screen.Update();
            //  game.context.ClearDepthStencilView(game.shadowView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            // game.context.OutputMerger.SetTargets(game.shadowView, game.renderView);

            // FrontBackRender(sphere);

            //screen.ResterizeStage();
            // game.context.Draw(screen.verticesCount, 0);

            //Prresent all
            //  game.swapChain.Present(0, PresentFlags.None);
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
            //renderTargets[3] = game.renderView;
        }

        public override void FrontBackRender(GameModelComp component)
        {
            component.Update();
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, component.vbSize);
            game.context.InputAssembler.InputLayout = screen.layout;
            game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            game.context.VertexShader.Set(screen.vertexShader2);
            game.context.PixelShader.Set(screen.pixelShader2);
            game.context.InputAssembler.SetVertexBuffers(0, vertexBufBinding);

            game.context.PixelShader.SetShaderResource(1, diffuseResourceView);
            game.context.PixelShader.SetShaderResource(2, normalResourceView);
            game.context.PixelShader.SetShaderResource(3, positionResourceView);
            game.context.PixelShader.SetSampler(0, sampler);

            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.VertexShader.SetConstantBuffer(0, screen.constantBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(0, screen.constantBuffer);

            game.context.PixelShader.Set(null);
            game.context.Rasterizer.State = backRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState;
            game.context.OutputMerger.BlendState = blendState;
            component.Draw();

            component.Update();
           
            game.context.Rasterizer.State = frontRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState2;
            game.context.OutputMerger.BlendState = addBlendState;
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, component.vbSize);
            game.context.InputAssembler.InputLayout = screen.layout;
            game.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            game.context.VertexShader.Set(screen.vertexShader2);
            game.context.PixelShader.Set(screen.pixelShader2);
            game.context.InputAssembler.SetVertexBuffers(0, vertexBufBinding);

            game.context.PixelShader.SetShaderResource(1, diffuseResourceView);
            game.context.PixelShader.SetShaderResource(2, normalResourceView);
            game.context.PixelShader.SetShaderResource(3, positionResourceView);
            game.context.PixelShader.SetSampler(0, sampler);

            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.VertexShader.SetConstantBuffer(0, screen.constantBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(0, screen.constantBuffer);
            game.context.PixelShader.Set(component.pixelShader);

            component.ResterizeStage();
            game.context.Draw(component.verticesCount, 0);
            game.context.OutputMerger.BlendState = blendState;
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
        private void CreatePositionBuff()
        {
            positionBuf = new Texture2D(game.device, new Texture2DDescription
            {
                Height = game.Form.ClientSize.Height,
                Width = game.Form.ClientSize.Width,
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0)
            });

            posRenderTarget = new RenderTargetView(game.device, positionBuf);
            positionResourceView = new ShaderResourceView(game.device, positionBuf, new ShaderResourceViewDescription
            {
                Format = Format.R8G8B8A8_UNorm,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });
        }
        private void CreateDiffuseBuf()
        {
            diffuseBuf = new Texture2D(game.device, new Texture2DDescription
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

            diffuseRenderTarget = new RenderTargetView(game.device, diffuseBuf);
            diffuseResourceView = new ShaderResourceView(game.device, diffuseBuf, new ShaderResourceViewDescription
            {
                Format = Format.R32G32B32A32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });
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

            normalRenderTarget = new RenderTargetView(game.device, normalBuf);
            normalResourceView = new ShaderResourceView(game.device, normalBuf, new ShaderResourceViewDescription
            {
                Format = Format.R32G32B32A32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });
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

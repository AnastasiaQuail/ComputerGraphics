using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

namespace GameFramework
{
    public class Render
    {
        Game game;
        public RasterizerStateDescription stateDescription;
        public  RasterizerState frontRasterizer, backRasterizer;
        public  DepthStencilState depthStencilState;
        public  BlendState blendState;
        public RenderTargetBlendDescription renderTargetBlendDescription;
        public BlendStateDescription blandStateDescr;
        public RasterizerStateDescription rasterStateDesc;
        public RasterizerState backRasterState;
        public SharpDX.Direct3D11.Device device;
        public SharpDX.Direct3D11.Buffer lightBuffer;
        public Texture2D shadowBuffer;
        public DepthStencilView shadowView;
        public RenderTargetBlendDescription blendStateDescription;
        public BlendState addBlendState;
        public DepthStencilStateDescription stencilStateDescription;
        public DepthStencilState depthState;
        public ShaderResourceView shadowResourceView;
        public DepthStencilState depthStencilState2;
        public DepthStencilState depthStateNoZBuf;

        public DepthStencilState depthStateZBuf { get;set; }

        public BlendState addblendState;

        public void Initialize(Game game)
        {
            this.game = game;
            stateDescription = new RasterizerStateDescription
            {
                CullMode = CullMode.Back,
                FillMode = FillMode.Solid
            };
            backRasterizer = new RasterizerState(game.device, stateDescription);
            stateDescription.CullMode = CullMode.Front;
            frontRasterizer = new RasterizerState(game.device, stateDescription);
            depthStencilState = new DepthStencilState(game.device, new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.Zero,
                IsStencilEnabled = true,
                DepthComparison = Comparison.Greater,
                FrontFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Equal,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep
                },
                BackFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.DecrementAndClamp,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep
                }
            });
            depthStencilState2 = new DepthStencilState(game.device, new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero,
                IsStencilEnabled = true,
                DepthComparison = Comparison.GreaterEqual,
                FrontFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Equal,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep
                },
                BackFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.DecrementAndClamp,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep
                }
            });
            depthStateNoZBuf = new DepthStencilState(game.device, new DepthStencilStateDescription
            {
                IsDepthEnabled = false
            });
            depthStateZBuf = new DepthStencilState(game.device, new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = false,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                FrontFace = { Comparison = Comparison.Always, DepthFailOperation = StencilOperation.Keep, FailOperation = StencilOperation.Keep, PassOperation = StencilOperation.Keep },
                BackFace = { Comparison = Comparison.Always, DepthFailOperation = StencilOperation.Keep, FailOperation = StencilOperation.Keep, PassOperation = StencilOperation.Keep },

            });
            blandStateDescr = new BlendStateDescription();
            renderTargetBlendDescription = new RenderTargetBlendDescription(true, BlendOption.One, BlendOption.One, BlendOperation.Add, BlendOption.Zero, BlendOption.Zero, BlendOperation.Add, ColorWriteMaskFlags.All);
            blandStateDescr.RenderTarget[0] = renderTargetBlendDescription;
            addblendState = new BlendState(game.device, blandStateDescr);
            blendState = new BlendState(game.device, new BlendStateDescription());
            blendStateDescription = new RenderTargetBlendDescription
            {
                IsBlendEnabled = false
            };
            blendState.Description.RenderTarget[0] = blendStateDescription;

        }

        public virtual void goRender()
        {
          
        }
        public virtual void FrontBackRender(GameModelComp component)
        {
            component.Update();
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, component.vbSize);
            game.context.PixelShader.Set(null);
            game.context.Rasterizer.State = backRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState;
            game.context.OutputMerger.BlendState = blendState;
            component.Draw();

            component.Update();
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, component.vbSize);
            game.context.PixelShader.Set(component.pixelShader);
            game.context.Rasterizer.State = frontRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState2;
            game.context.OutputMerger.BlendState = addBlendState;
            component.UpdateContext(PrimitiveTopology.TriangleList, Utilities.SizeOf<Vector4>() * 2);
            game.context.PixelShader.SetShaderResource(0, null);
            game.context.PixelShader.SetShaderResource(1,null);
            game.context.VertexShader.SetConstantBuffer(0, component.constantBuffer);
            game.context.VertexShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
            game.context.InputAssembler.SetVertexBuffers(0, component.BufferBinding);
            component.ResterizeStage();
            game.context.Draw(component.verticesCount, 0);
            game.context.OutputMerger.BlendState = blendState;
        }
    }
}

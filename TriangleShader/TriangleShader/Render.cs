using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    class Render
    {
        Game game;
        RasterizerStateDescription stateDescription;
        RasterizerState frontRasterizer,backRasterizer;
        DepthStencilState depthStencilState;
        BlendState blendState;

        public Render(Game gameObj)
        {
            this.game = gameObj;
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
                FrontFace =
                {
                    Comparison = Comparison.Equal,
                    PassOperation = StencilOperation.Keep
                },
                BackFace =
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.DecrementAndClamp
                }
            });
            //blendState = new BlendState(game.device,new BlendStateDescription { IndependentBlendEnable = true});
            //blendState.Description.RenderTarget =new RenderTargetBlendDescription(true,BlendOption.One,BlendOption.One,BlendOperation.Add,BlendOption.Zero,BlendOption.Zero,BlendOperation.Add,ColorWriteMaskFlags.All);
        }
        public void FrontBackRender(GameComponent component)
        {
            component.Update();
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, Utilities.SizeOf<Vector4>() * 3);
            game.context.PixelShader.Set(null);
            game.context.Rasterizer.State = backRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState;
            component.Draw();

            component.Update();
            component.UpdateContext(SharpDX.Direct3D.PrimitiveTopology.TriangleList, Utilities.SizeOf<Vector4>() * 3);
            game.context.PixelShader.Set(null);
            game.context.Rasterizer.State = backRasterizer;
            game.context.OutputMerger.DepthStencilState = depthStencilState;
            component.Draw();

        }
    }
}

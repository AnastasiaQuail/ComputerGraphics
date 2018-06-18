using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    class ForwardRenderer:Render
    {
        Game game;
        public ForwardRenderer(Game gameObj)
        {
            game = gameObj;

        }
        public override void goRender()
        {
            game.context.ClearDepthStencilView(game.depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            game.context.ClearRenderTargetView(game.renderView, Color.DarkSlateBlue);
            game.context.OutputMerger.SetTargets(game.depthView, game.renderView);

            foreach (var component in game.Components)
            {
                //Update components
                component.Update();
            }

            //Drawing all components of the Game
            foreach (var component in game.Components)
            {
                component.Draw();
            }

            //Prresent all
            game.swapChain.Present(0, PresentFlags.None);
        }


    }
}

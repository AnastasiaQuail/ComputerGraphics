using SharpDX;
using System;

namespace GameFramework
{
	public class LightComponent
    {
        LightData light;
        Game game;

        public LightComponent(Game game,Vector4 position, Vector4 color)
        {
            light.Color = color;
            light.Position = position;
            this.game = game;
        }
        public void Update()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4(50f, (float)Math.Cos(time) * 50f, 0f, 0f);
           // game.context.UpdateSubresource(ref light, lightBuffer);
            game.context.PixelShader.SetConstantBuffer(1, game.lightBuffer);
        }
        
       


    }
}

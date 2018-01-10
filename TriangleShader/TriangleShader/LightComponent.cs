using SharpDX;
using System;

namespace GameFramework
{
	public class LightComponent
    {
        public LightData light;
        Game game;

        public LightComponent(Game game,Vector4 position, Vector4 color)
        {
            light.Color = color;
            light.Position = position;
            this.game = game;
        }
        public LightComponent(Game game)
        {
            light = new LightData();
            this.game = game;
        }
        public virtual void Update()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4(50f, (float)Math.Cos(time) * 50f, 0f, 0f);
            game.context.UpdateSubresource(ref light, game.lightBuffer);
        }
        
       


    }
}

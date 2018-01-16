using SharpDX;
using System;

namespace GameFramework
{
    public class LightCamera : GameComponent
    {
        public LightData light;
        Matrix ViewMatrix, ProjMatrix, ViewProj, RotationMatrix;
        Vector3 position, lookAt, up;
        public Camera camera { get; set; }

        public LightCamera(Game game)
        {
            this.game = game;
            camera = new Camera(game, (Vector3)(light.Position), true);
        }
        
        public void setLightData(Vector4 position, Vector4 color)
        {
            light.Color = color;
            light.Position = position;
            camera.GetVPMatrix(out light.ViewProj);
        }
        public override void Update()
        {
            updateLightData();
            game.context.UpdateSubresource(ref light, game.lightBuffer);
        }
        public virtual void updateLightData()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4(100f, (float)Math.Cos(time) * 50f, 0f, 0f);
            camera.SetPosition(light.Position.X, light.Position.Y, light.Position.Z);
            camera.GetVPMatrix(out light.ViewProj);
        }
        public override void Initialize(Game game, string nameOfFile, bool IsLight)
        { 
        }
    }
}

using SharpDX;
using System;

namespace GameFramework
{
    public class LightCamera : GameComponent
    {
        public LightData light;
        public Camera camera { get; set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjMatrix { get; private set; }

        public LightCamera(Game game)
        {
            this.game = game;
            lightFlag = false;
            camera = new Camera(game, (Vector3)(light.Position), true);  //добавить направление света
            camera.setLookAt(new Vector3(0, 0, 0));
        }
        
        public void setLightData(Vector4 position, Vector4 color)
        {
            light.Color = color;
            light.Position = position;
            camera.SetPosition(position.X, position.Y, position.Z);
            camera.GetVPMatrix(out light.ViewProj);
        }
        public override void Update()
        {
            //ViewMatrix = Matrix.LookAtLH(camera.getPosition(), camera.lookAt, camera.rotationMatrix.Up);
            //ProjMatrix = Matrix.OrthoLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);
            //light.ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);
            updateLightData();
            game.context.UpdateSubresource(ref light, game.lightBuffer);
        }
        public virtual void updateLightData()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4(0f, (float)Math.Cos(time) * 10f, 5f, 1f);
            camera.SetPosition(light.Position.X, light.Position.Y, light.Position.Z);///нормальное обновление направления камеры
            camera.GetVPMatrix(out light.ViewProj);
        }
        public override void Initialize(Game game, string nameOfFile, bool IsLight)
        { 
        }
    }
}

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
        public Matrix ViewProj { get; private set; }

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
            updateLightData();
            game.context.UpdateSubresource(ref light, game.lightBuffer);
        }
        public virtual void updateLightData()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4( 0f, (float)Math.Cos(time) * 5f, 5f, 1f);
            camera.SetPosition(light.Position.X, light.Position.Y, light.Position.Z);///нормальное обновление направления камеры
            //camera.GetVPMatrix(out light.ViewProj);
            ViewMatrix = Matrix.LookAtLH(camera.getPosition(), camera.getLookAt(), camera.rotationMatrix.Up);
            ProjMatrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);
            light.ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);
        }
        public override void Initialize(Game game, string nameOfFile, bool IsLight)
        { 
        }
        public void RenderLight()
        {
            camera.UpdateRotationMatrix();
            camera.lookAt = camera.getPosition() + camera.rotationMatrix.Forward;

            Vector3 up = camera.rotationMatrix.Up;

            // Finally create the view matrix from the three updated vectors.
            ViewMatrix = Matrix.LookAtLH(camera.getPosition(), camera.lookAt, up);
            ProjMatrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);

            ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);
        }
    }
}

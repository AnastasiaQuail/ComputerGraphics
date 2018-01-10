using SharpDX;
using System;

namespace GameFramework
{
    public class LightCamera:GameComponent
    {
        public LightData light;
        Matrix ViewMatrix,ProjMatrix,ViewProj,RotationMatrix;
        Vector3 position, lookAt,up;

        public LightCamera(Game game)
        {
            this.game = game;
            RotationMatrix = Matrix.RotationYawPitchRoll(0,0,0);
            position = (Vector3)(light.Position);
            lookAt = position + RotationMatrix.Forward;
            up = RotationMatrix.Up;

            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
            ProjMatrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);

            ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);
		    //Initialize(game, "Shaders/Shadow.fx",false);

        }
		
        public void GetVPMatrix(out Matrix VPMatrix)
        {
            VPMatrix = ViewProj;
        }
        public void setLightData(Vector4 position, Vector4 color)
        {
            light.Color = color;
            light.Position = position;
        }
        public override void Update()
        {
            updateLightData();
            game.context.UpdateSubresource(ref light, game.lightBuffer);
        }
        public virtual void updateLightData()
        {
            var time = game.clock.ElapsedMilliseconds * 0.001f;
            light.Position = new Vector4(50f, (float)Math.Cos(time) * 50f, 0f, 0f);
        }
    }
}

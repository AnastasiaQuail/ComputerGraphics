using SharpDX;

namespace GameFramework
{
    class LightComponent2:GameComponent
    {
        Matrix ViewMatrix,ProjMatrix,ViewProj,RotationMatrix;
        Vector3 position, lookAt,up;

        public LightComponent2(Game game)
        {
            RotationMatrix = Matrix.RotationYawPitchRoll(0,0,0);
            position = (Vector3)(game.lightData.Position);
            lookAt = position + RotationMatrix.Forward;
            up = RotationMatrix.Up;

            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
            ProjMatrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);

            ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);
			this.game = game;
			Initialize(game, "Shaders/Shadow.fx",false);

        }
		
        public void GetVPMatrix(out Matrix VPMatrix)
        {
            VPMatrix = ViewProj;
        }
    }
}

using System.Collections.Generic;
using Vector4 = SharpDX.Vector4;
using SharpDX.Direct3D;

namespace GameFramework
{


    public class Cells : GameComponent
    {
        public Cells(Game game)
        {
            this.game = game;
            Initialize(game, "BCTexture.fx",false);
        }

        public override void Draw()
        {
            UpdateContext(PrimitiveTopology.LineList);
           
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(320, 0);
        }

        public new Vector4[] AIStage()
        {
            List<Vector4> pointsList = new List<Vector4>();
            for (int i = -500; i < 500; i+=10)
            {
                pointsList.Add(new Vector4(-500f, 0f, i, 1f));
                pointsList.Add(new Vector4(0f, 0f, 1f, 1f));
                pointsList.Add(new Vector4(500f, 0f, i, 1f));
                pointsList.Add(new Vector4(0f, 0f, 1f, 1f));

                pointsList.Add(new Vector4(i, 0f, -500f, 1f));
                pointsList.Add(new Vector4(0f, 0f, 1f, 1f));
                pointsList.Add(new Vector4(i, 0f, 500f, 1f));
                pointsList.Add(new Vector4(0f, 0f, 1f, 1f));
            }

            return pointsList.ToArray();
        }
    }
}

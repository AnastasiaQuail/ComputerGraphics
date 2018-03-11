using System.Collections.Generic;
using Vector4 = SharpDX.Vector4;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;

namespace GameFramework
{


    public class Cells : GameComponent
    {
        public Cells(Game game)
        {
            this.game = game;
            transform = new Transform();
            Initialize(game, "Shaders/BCTriangle.fx", false);
        }

        public override void Draw()
        {
            UpdateContext(PrimitiveTopology.LineList, Utilities.SizeOf<Vector4>() * 2);

            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(320, 0);
        }

        public override Points[] AIStage()
        {
            List<Points> pointsList = new List<Points>();
            for (int i = -500; i < 500; i += 10)
            {
                pointsList.Add(new Points(new Vector4(-500f, 0f, i, 1f), new Vector4(0, 1, 0.2f, 0)));
                pointsList.Add(new Points(new Vector4(500f, 0f, i, 1f), new Vector4(0, 1, 0.2f, 0)));

                pointsList.Add(new Points(new Vector4(i, 0f, -500f, 1f), new Vector4(0, 1, 0.2f, 0)));
                pointsList.Add(new Points(new Vector4(i, 0f, 500f, 1f), new Vector4(0, 1, 0.2f, 0)));
            }

            return pointsList.ToArray();
        }
        public override void CreateLayout()
        {
            //Create layout
            layout = new InputLayout(device,
               ShaderSignature.GetInputSignature(vertexShaderBC),
               new[]
               {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
               });
        }
    }
}

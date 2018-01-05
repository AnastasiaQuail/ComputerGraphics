using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class SurfaceComponent:GameComponent
    {

        Stopwatch clock;
        public float width, heigh;
        public float z;
        Vector4 color;
        public Vector3 startPos;

        public SurfaceComponent(Game gameObj,float width, float heigh, Vector4 color)
        {
            this.game = gameObj;
            clock = game.clock;
            this.width = width/2f;
            this.heigh = heigh/2f;
            this.color = color;
            z = 0;
            Initialize(game, "Shaders/BCTexture.fx",false);

        }

        public override Points[] AIStage()
        {
            return new Points[]
            {
                new Points( new Vector4(-width, -heigh, z, 1.0f), new Vector4(0,1,0,0)),
				new Points(new Vector4(-width,  heigh, z, 1.0f), new Vector4(0,0,0,0)),
				new Points(new Vector4( width,  heigh, z, 1.0f), new Vector4(1,0,0,0)),
                new Points(new Vector4(-width, -heigh, z, 1.0f), new Vector4(0,1,0,0)),
                new Points(new Vector4( width,  heigh, z, 1.0f), new Vector4(1,0,0,0)),
				new Points(new Vector4( width, -heigh, z, 1.0f), new Vector4(1,1,0,0))
			};
        }
        public override void Draw()
        {
			base.Draw();
            UpdateContext(PrimitiveTopology.TriangleList);
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(6, 0);
        }

        
    }

}

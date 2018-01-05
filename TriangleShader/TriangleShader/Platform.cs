using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class Platform:SurfaceComponent
    {
        public PlayerController controller;
        public Pong gamePong;
        public Platform(Pong gameObj, float width, float heigh, Vector4 color):base(gameObj,width,heigh,color)
        {
            gamePong = gameObj;
            z = 10;
            startPos = new Vector3(transform.Position.X, transform.Position.Y, z);
            SetTextureFile("PlatformTexture.jpg");
        }
        public void SetCenter(Vector2 pos) { transform.Position = new Vector3(pos.X, pos.Y, z); startPos = transform.Position; }
        public void SetController(PlayerController playerController){ controller = playerController; }

        public override void Update()
        {
            controller.Control();
            controller.PushBall();
            base.Update();

        }
    }
}

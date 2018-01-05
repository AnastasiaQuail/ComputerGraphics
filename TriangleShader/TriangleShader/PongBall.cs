using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameFramework
{
    public class PongBall: SurfaceComponent
    {
        Pong gamePong;
        public Vector3 forward;
        public PongBall(Pong gameObj, float width, float heigh, Vector4 color) :base(gameObj,width,heigh,color)
        {
            this.gamePong = gameObj;
            z = 20;
            startPos = new Vector3(transform.Position.X, transform.Position.Y, z);
            forward = new Vector3(0, 1, 0);
            System.Console.WriteLine("Platform texture "+File.Exists("Platform.jpg"));
            SetTextureFile("Texture.jpg");
            

        }
        public void Start()
        {
            transform.SetFrameTime(game.clock.ElapsedMilliseconds);
            transform.MoveVector(forward);
            
        }
        public override void Update()
        {
            Push();
            base.Update();

        }
        public bool IsOnBorder()
        {
            if (Math.Abs(transform.Position.X) > (gamePong.GetFieldSize().X - width*2))
            {
                if (transform.Position.X < 0)
                    forward = Vector3.Reflect(forward, new Vector3(1, 0, 0));
                else
                    forward = Vector3.Reflect(forward, new Vector3(-1, 0, 0));
                return true;
            }
            return false;
        }
        public bool IsOut()
        {
            if ((Math.Abs(transform.Position.Y) - heigh > gamePong.GetFieldSize().Y))
            {
                return true;
            }
            return false;
        }
        public void Push()
        {
            IsOnBorder();
            if (!IsOut())
                transform.MoveVector(this.forward);
           else
                gamePong.SetScore();
        }

    }
}

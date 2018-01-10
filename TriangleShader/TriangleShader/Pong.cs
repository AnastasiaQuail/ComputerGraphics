using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameFramework
{
    public class Pong:Game
    {
        SurfaceComponent surface;
        public Platform surface1;
        public Platform surface2;
        public PongBall ball;
        public Vector2 score;
        

        public bool flagOutside;
        public Pong(string name, int fwidth, int fheigh, Vector3 position) : base(name, fwidth, fheigh, position,true)
        {
			mycamera.setLookAt(new Vector3(0,0,-15f));
            score = new Vector2();
            flagOutside = false;
            Components = new List<GameComponent>();
            sceneLight = new LightComponent(this);
            sceneLight.light.Position = new Vector4(0, 20, 0, 1);
            sceneLight.light.Color = new Vector4(0, 20, 0, 1);

			ball = new PongBall(this, 10, 10, new Vector4(0.2f, 1f, 0.5f, 1f));
            
           surface = new SurfaceComponent(this,200,220,new Color4(0f,0.5f,1f,1f));
           surface.SetTextureFile("Moon.jpg");

           surface1 = new Platform(this,30,10,new Color4(1f,1f,1f,1f));
           surface2 = new Platform(this,30,10,Color4.White);
           surface1.SetCenter(new Vector2(0, 100));
           surface2.SetCenter(new Vector2(0, -100));
           Components.Add(surface);
           PlayerController player1 = new PlayerController(this, surface1, Keys.A, Keys.D);
            PlayerController player2 = new PlayerController(this, surface2, Keys.Left, Keys.Right);
            surface1.SetController(player1);
            surface2.SetController(player2);

           Components.Add(surface1);
           Components.Add(surface2);
            Components.Add(ball);
        }
        public Vector2 GetFieldSize()
        {
            return new Vector2(surface.width,surface.heigh);
        }
        public void CheckScore(){
            if ((score.X == 6) || (score.Y == 6))
                IsActive = false;
        }
        public void SetScore()
        {
            if (ball.transform.Position.Y < 0)
            {
                score.Y += 1;
            }
            else
            {
                score.X += 1;
            }
            StartScreen();
            CheckScore();
        }
        public void StartScreen()
        {
            System.Threading.Thread.Sleep(300);
            surface1.transform.SetPosition(surface1.startPos);
            surface2.transform.SetPosition(surface2.startPos);
            ball.transform.SetPosition(ball.startPos);
            ball.forward = new Vector3(0, 1, 0);
        }
        
    }
}

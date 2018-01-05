using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Mathematics;

namespace GameFramework
{
    public class PlayerController
    {
        InputDevice input;
        Transform transform;
        Keys right, left;
        Platform component;
        Vector3 forward;
        PongBall ball;

        public PlayerController(Game game,Platform component, Keys right, Keys left)
        {
            input = game.inputDevice;
            this.component = component;
            transform = component.transform;
            this.right = right;
            this.left = left;
            ball = component.gamePong.ball;
            forward = (ball.transform.Position - component.transform.Position); ;
        }
        public void Control()
        {
            forward= (ball.transform.Position - component.transform.Position);
            if (input.IsKeyDown(right)) //Move Right
            {
                transform.Position += new Vector3(1, 0, 0) * transform.delta*5f;
                if (transform.Position.X > 90)
                    transform.Position.X = 90;
            }

            if (input.IsKeyDown(left)) //Move Left 
            {
                transform.Position -= new Vector3(1, 0, 0) * transform.delta*5f;
                if (transform.Position.X < -90)
                    transform.Position.X = -90;
            }
            
        }
        public bool IsColide()
        {
            forward = (ball.transform.Position - component.transform.Position);
            if (Math.Abs(ball.transform.Position.Y-component.transform.Position.Y) <= (component.heigh+ball.heigh) &
                (Math.Abs(ball.transform.Position.X - component.transform.Position.X)) <= Math.Abs(component.width+ball.width))
            {
                ball.forward = new Vector3(forward.X,forward.Y,0);
                return true;
            }
            return false;
        }
        public void PushBall()
        {
            IsColide();
        }
        
    }
}

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.RawInput;
using SharpDX.Multimedia;
using System.Windows.Forms;
using System.Diagnostics;

namespace GameFramework
{
    public class Camera
    {
        private float pitch { get; set; }
        private float yaw { get; set; }
        private float roll { get; set; }
        private Vector3 Position;
        public Matrix ViewMatrix { get; private set;}
        public Matrix ProjMatrix { get; set; }
        public Matrix ViewProj { get; set; }
        public Game game;
        private InputDevice input;
        private Transform transphorm;
        private SharpDX.Vector2 posMouse;
        private Stopwatch clock;
        private float frameTime;
        public float delta;
        public Matrix rotationMatrix;
        public Vector3 lookAt { get; set; }
        private bool flag;


        // Constructor
        public Camera(Game game, Vector3 position, bool staticFlag)
        {
            this.game = game;

            input = game.inputDevice;
            transphorm = new Transform();
            SetPosition(position.X,position.Y,position.Z);
            clock = game.clock;
            frameTime = clock.ElapsedMilliseconds;
            
            // Setup the position of the camera in the world.
            Position = position;
            // Setup where the camera is looking by default.
            lookAt = new Vector3(0f, 0f, 0f);
            UpdateRotationMatrix();

            if (!staticFlag)
            {
                input.MouseMove += MouseControl;
            }

            flag = staticFlag;
            
        }

        // Methods.
        public void SetPosition(float x, float y, float z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
           
        }
        public Vector3 getPosition() { return Position; }
        
        public void Render()
        {

            //delta time
            var currTime = clock.ElapsedMilliseconds;
            delta = (currTime - frameTime) * 0.01f;

            if (!flag)
            {
                //Update transformations
                Keyboard_Controls();
            }
            //update time
            frameTime = currTime;

            UpdateRotationMatrix();
            lookAt = Position + rotationMatrix.Forward;
            
            Vector3 up = rotationMatrix.Up ;

            // Finally create the view matrix from the three updated vectors.
            ViewMatrix = Matrix.LookAtLH(Position, lookAt, up);
            ProjMatrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(90), 1f, 0.1f, 1000f);
            
            ViewProj = Matrix.Multiply(ViewMatrix, ProjMatrix);

        }
        private void Keyboard_Controls()
        {

            if(input.IsKeyDown(Keys.W)) //Move Forward
            {
               Position +=rotationMatrix.Forward*delta;   
            }
            
            if (input.IsKeyDown(Keys.S)) //Move Backward
            {
                Position -= rotationMatrix.Forward * delta; 
            }

            if (input.IsKeyDown(Keys.A)) //Move Left
            {
                Position -= rotationMatrix.Left*delta;
            }

            if (input.IsKeyDown(Keys.D)) //Move Right
            {
                Position -= rotationMatrix.Right * delta;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Position += rotationMatrix.Up*delta;
            }
            if (input.IsKeyDown(Keys.X))
            {
                Position -= rotationMatrix.Up*delta;
            }
            
        }
        private void MouseControl(InputDevice.MouseMoveEventArgs eventArgs)
        {
           
            Vector2 curentMPos = input.MousePositionLocal;

            yaw += MathUtil.DegreesToRadians(eventArgs.Offset.X) * delta*5f;
            pitch -= MathUtil.DegreesToRadians(eventArgs.Offset.Y) * delta*5f;

            if (Math.Abs(pitch) > 60)
                pitch = 60f*Math.Sign(pitch);
            if (Math.Abs(yaw) > 180)
                yaw = 180f*Math.Sign(yaw);

            posMouse = curentMPos;
           
        }
     
        public void GetVPMatrix(out Matrix VPMatrix)
        {
            VPMatrix= ViewProj;
        }
        private void UpdateRotationMatrix()
        {
            // Create the rotation matrix from the yaw, pitch, and roll values.
            rotationMatrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);
        }
     public Matrix getRotationM()
        {
            return rotationMatrix;
        }
		public void setLookAt(Vector3 look)
		{
			lookAt = look;
		}


    }
}

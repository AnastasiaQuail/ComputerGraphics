using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class Transform
    {
    
            // Variables
            float m_FrameTime;
            float m_ForwardSpeed, m_BackwardSpeed;
            float m_UpwardSpeed, m_DownwardSpeed;
            float m_LeftTurnSpeed, m_RightTurnSpeed;
            float m_LookUpSpeed, m_LookDownSpeed;
        
        public float delta;
        Matrix WorldMatrix;

            // Properties
            public Vector3 Position;
            public float RotationX { get; set; }
            public float RotationY { get; set; }
            public float RotationZ { get; set; }
        public float Scale;

            public  float Rotation { get; set; }
        public Transform()
        {
            Scale = 1;
            WorldMatrix = Matrix.Identity;
            delta =  0.08f;
        }

        // Public Methods
        public void SetPosition(float x, float y, float z)
            {
                Position.X = x;
                Position.Y = y;
                Position.Z = z;
            }
        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }
        public Vector3 GetPosition()
        {
            return new Vector3(Position.X,Position.Y,Position.Z);
        }
        public void SetRotation(float x, float y, float z)
            {
                RotationX = x;
                RotationY = y;
                RotationZ = z;
            }
            public void SetFrameTime(float time)
            {
            m_FrameTime = time;
            delta = time*0.00001f;
            }
            public void MoveForward(bool keydown)
            {
                // Update the forward speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_ForwardSpeed += m_FrameTime * 0.001f;
                    if (m_ForwardSpeed > m_FrameTime * 0.03)
                        m_ForwardSpeed = m_FrameTime * 0.03f;
                }
                else
                {
                    m_ForwardSpeed -= m_FrameTime * 0.0007f;
                    if (m_ForwardSpeed < 0)
                        m_ForwardSpeed = 0;
                }

                // Convert degrees to radians.
                float radians = RotationY * 0.0174532925f;

                // Update the position.
                Position.X += (float)Math.Sin(radians) * m_ForwardSpeed;
                Position.Z += (float)Math.Cos(radians) * m_ForwardSpeed;
            }
            public void MoveBackward(bool keydown)
            {
                // Update the backward speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_BackwardSpeed += m_FrameTime * 0.001f;
                    if (m_BackwardSpeed > m_FrameTime * 0.03f)
                        m_BackwardSpeed = m_FrameTime * 0.03f;
                }
                else
                {
                    m_BackwardSpeed -= m_FrameTime * 0.0007f;
                    if (m_BackwardSpeed < 0)
                        m_BackwardSpeed = 0;
                }

                // Convert degrees to radians.
                float radians = RotationY * 0.0174532925f;

                // Update the position.
                Position.X -= (float)Math.Sin(radians) * m_BackwardSpeed;
                Position.Z -= (float)Math.Cos(radians) * m_BackwardSpeed;
            }
            public void MoveUpward(bool keydown)
            {
                // Update the downward speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_UpwardSpeed += m_FrameTime * 0.003f;
                    if (m_UpwardSpeed > (m_FrameTime * 0.03f))
                        m_UpwardSpeed = m_FrameTime * 0.03f;
                }
                else
                {
                    m_UpwardSpeed -= m_FrameTime * 0.002f;

                    if (m_UpwardSpeed < 0.0f)
                        m_UpwardSpeed = 0.0f;
                }

                // Update the height position.
                Position.Y += delta;
            }
            public void MoveDownward(bool keydown)
            {
                // Update the downward speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_DownwardSpeed += m_FrameTime * 0.003f;
                    if (m_DownwardSpeed > (m_FrameTime * 0.03f))
                        m_DownwardSpeed = m_FrameTime * 0.03f;
                }
                else
                {
                    m_DownwardSpeed -= m_FrameTime * 0.002f;
                    if (m_DownwardSpeed < 0.0f)
                        m_DownwardSpeed = 0.0f;
                }

                // Update the height position.
                Position.Y -= m_DownwardSpeed;
            }
            public void TurnLeft(bool keydown)
            {
                // Update the left turn speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_LeftTurnSpeed += m_FrameTime * 0.01f;
                    if (m_LeftTurnSpeed > m_FrameTime * 0.15)
                        m_LeftTurnSpeed = m_FrameTime * 0.15f;
                }
                else
                {
                    m_LeftTurnSpeed -= m_FrameTime * 0.005f;
                    if (m_LeftTurnSpeed < 0)
                        m_LeftTurnSpeed = 0;
                }

                // Update the rotation using the turning speed.
                RotationY -= m_LeftTurnSpeed;

                // Keep the rotation in the 0 to 360 range.
                if (RotationY < 0)
                    RotationY += 360;
            }
            public void TurnRight(bool keydown)
            {
                // Update the right turn speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_RightTurnSpeed += m_FrameTime * 0.01f;
                    if (m_RightTurnSpeed > m_FrameTime * 0.15)
                        m_RightTurnSpeed = m_FrameTime * 0.15f;
                }
                else
                {
                    m_RightTurnSpeed -= m_FrameTime * 0.005f;
                    if (m_RightTurnSpeed < 0)
                        m_RightTurnSpeed = 0;
                }

                // Update the rotation using the turning speed.
                RotationY += delta*0.1f;

                // Keep the rotation in the 0 to 360 range which is looking stright Up.
                if (RotationY > 360)
                    RotationY -= 360;
            }
        public void LookUpward(bool keydown)
        {
            // Update the upward rotation speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                m_LookUpSpeed += m_FrameTime * 0.01f;
                if (m_LookUpSpeed > m_FrameTime * 0.15)
                    m_LookUpSpeed = m_FrameTime * 0.15f;
            }
            else
            {
                m_LookUpSpeed -= m_FrameTime * 0.005f;
                if (m_LookUpSpeed < 0)
                    m_LookUpSpeed = 0;
            }

            // Update the rotation using the turning speed.
            RotationX -= m_LookUpSpeed;

            // Keep the rotation maximum 90 degrees.
            if (RotationX > 90)
                RotationX = 90;
        }
        public void LookDownward(bool keydown)
            {
                // Update the downward rotation speed movement based on the frame time and whether the user is holding the key down or not.
                if (keydown)
                {
                    m_LookDownSpeed += m_FrameTime * 0.01f;
                    if (m_LookDownSpeed > m_FrameTime * 0.15)
                        m_LookDownSpeed = m_FrameTime * 0.15f;
                }
                else
                {
                    m_LookDownSpeed -= m_FrameTime * 0.005f;
                    if (m_LookDownSpeed < 0)
                        m_LookDownSpeed = 0;
                }

                // Update the rotation using the turning speed.
                RotationX += m_LookDownSpeed;

                // Keep the rotation maximum 90 degrees which is looking straight down.
                if (RotationX < -90)
                    RotationX = -90;
            }
        // Static Methods
        public  float Rotate()
        {
            Rotation += delta*0.05f;

            if (Rotation > 360)
                Rotation -= 360;
            return Rotation;
        }

        public void MoveRight()
        {
            Position.X +=   delta;
        }

        public void MoveLeft()
        {
            Position.X -=  delta;
        }
        public Matrix GetWorldMatrix()
        {
            
         //   if ((this.Position == new Vector3(0, 0, 0)) && (RotationX == 0) && (RotationY == 0))
           //     return Matrix.Identity;
           // else
            {
                //
                WorldMatrix = Matrix.Scaling(Scale) * Matrix.RotationX(RotationX) * Matrix.RotationY(RotationY) * Matrix.RotationZ(RotationZ) * Matrix.Translation(Position.X, Position.Y, Position.Z);
                return WorldMatrix;
            }
        }

        public void MoveVector(Vector3 forward)
        {
            Position += forward * delta*0.4f;
        }
        public void GoCircle(Vector3 radius)
        {
            RotationY += delta;
          // WorldMatrix = Matrix.Translation(-radius) * Matrix.RotationY(RotationY) * Matrix.Translation(radius);
        }
    }
}


using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Vector4 = SharpDX.Vector4;

namespace GameFramework
{
    public class CubeComponent: GameComponent
    {


        public CubeComponent(Game gameObj)
        {
            // Use clock
            this.game = gameObj;
            textureFile = "Texture.jpg";
            nameOfShader = "Shaders/BCTexture.fx";
            Initialize(game, "Shaders/BCTexture.fx",false);
            transform = new Transform();
            WorldViewProjMatrix = Matrix.Identity;
        }

        public override Points[] AIStage()
        {
            return new Points[] {
                                     new Points( new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4( 0.0f, 1.0f, 0.0f, 1.0f)), // Front
                                     new Points( new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f)),

                                     new Points( new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)), // BACK
                                      new Points(new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                                      new Points(new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4( 1.0f, 1.0f, 0.0f, 1.0f)),
                                      new Points(new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),

                                     new Points( new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)), // Top
                                     new Points( new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)),

                                     new Points( new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)), // Bottom
                                     new Points( new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),

                                     new Points( new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)), // Left
                                     new Points( new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)),

                                     new Points( new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)), // Right
                                     new Points( new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                                     new Points( new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                            };

        }
        public override void Update()
        {
  
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            WorldMatrix = transform.GetWorldMatrix();
           // Transformation();

            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);

            //Set data
            InvertWorld = WorldMatrix;
            InvertWorld.Invert();

            constantData.World = WorldMatrix;
            constantData.InvertWorld = InvertWorld;
            constantData.WorldViewProj = WorldViewProjMatrix;
            constantData.ViewPos = new Vector4(transform.Position, 1);

            game.context.UpdateSubresource(ref constantData, constantBuffer);


        }

        public virtual void Transformation()
        {
            transform.GoCircle(new Vector3(2, 0, 0));
            
        }

        public override void Draw()
        {
            base.Draw();
            UpdateContext(PrimitiveTopology.TriangleList);
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);

            ResterizeStage();
            game.context.Draw(36, 0);
        }
        public override void ShadowDraw()
        {
            base.ShadowDraw();
            UpdateShadow();
            game.context.VertexShader.SetConstantBuffer(0, constantBuffer);
            ResterizeStage();
            game.context.Draw(36, 0);

        }


    }
}




////Set WorldViewProj()
//and in the MyGame class make transformation for every cube
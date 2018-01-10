using System.Collections.Generic;
using SharpDX;
using System;

namespace GameFramework
{
    public class MyGame:Game
    {
        CubeComponent cubeMini;
        public MyGame(string name, int fwidth, int fheigh, Vector3 position, bool flag):base( name,fwidth,fheigh,position,flag)
        {
            lightData = new LightData();
            lightData.Position = new Vector4(10, 10, 0, 1);
            lightData.Color =(Vector4) Color.Cyan;

            sceneLight = new LightComponent(this, lightData.Position, lightData.Color);

            Components = new List<GameComponent>();
            
            //CubeComponent cube = new CubeComponent(this);
            //  cube.SetWorldMatrix(Matrix.RotationY(60));
            //cube.transform.Scale=100f;
            //  Components.Add(cube);
            //  TriangleComponent triangle = new TriangleComponent(this);
            //SurfaceComponent surface = new SurfaceComponent(this, 50, 50, Color4.White);


            /*Components.Add(triangle);
             surface.SetTextureFile("Platform.jpg");
             Components.Add(surface);

             MoonCubeComponent moon = new MoonCubeComponent(this,cubeMini);
             moon.transform.Scale = 0.5f;
             moon.SetTextureFile("PlatformTexture.jpg");
             Components.Add(moon);

             MoonCubeComponent moon2 = new MoonCubeComponent(this, moon);
             moon2.transform.Scale = 0.5f;
             moon2.SetTextureFile("Moon.jpg");
             Components.Add(moon2);

             // Components.Add(lines);
             */
            cubeMini = new CubeComponent(this);
            cubeMini.transform.Scale = 2f;
            cubeMini.transform.Position = new Vector3(10,10,0);
            cubeMini.SetTextureFile("Moon.jpg");
            Components.Add(cubeMini);

            GameModelComp cow = new GameModelComp(this, "cow.obj");
            cow.SetTextureFile("cow.jpg");
            cow.transform.Scale = 10f;
            Components.Add(cow);



        }

       public void Transformation(CubeComponent cube, Vector3 radius)
        {
            var time = clock.ElapsedMilliseconds*0.01f;
            
           // cube.transform.MoveVector(new Vector3((float)Math.Cos(angle),(float)Math.Sin(angle),0f));
        }
        public override void UpdateLight()
        {
            var time = clock.ElapsedMilliseconds * 0.001f;
            lightData.Position =new Vector4(50f,(float)Math.Cos(time)*50f,0f,0f);
            cubeMini.transform.Position = new Vector3(50f, (float)Math.Cos(time)*50f, 0f);
        }
    }
}

using System.Collections.Generic;
using SharpDX;
using System;

namespace GameFramework
{
    public class MyGame:Game
    {

        public MyGame(string name, int fwidth, int fheigh, Vector3 position, bool flag):base( name,fwidth,fheigh,position,flag)
        {
            
           
           // Cells lines = new Cells(this);
           
            Components = new List<GameComponent>();
            //CubeComponent cube = new CubeComponent(this);
            //  cube.SetWorldMatrix(Matrix.RotationY(60));
            //cube.transform.Scale=100f;
            //  Components.Add(cube);
          //  TriangleComponent triangle = new TriangleComponent(this);
            SurfaceComponent surface = new SurfaceComponent(this, 50, 50, Color4.White);

           
           // Components.Add(triangle);
            surface.SetTextureFile("Platform.jpg");
            Components.Add(surface);

            CubeComponent cubeMini = new CubeComponent(this);
            cubeMini.transform.Scale = 2f;
            cubeMini.SetTextureFile("Texture.jpg");
            Components.Add(cubeMini);

            MoonCubeComponent moon = new MoonCubeComponent(this,cubeMini);
            moon.transform.Scale = 0.5f;
            moon.SetTextureFile("PlatformTexture.jpg");
            Components.Add(moon);

            MoonCubeComponent moon2 = new MoonCubeComponent(this, moon);
            moon2.transform.Scale = 0.5f;
            moon2.SetTextureFile("Moon.jpg");
            Components.Add(moon2);

            // Components.Add(lines);



        }

       public void Transformation(CubeComponent cube, Vector3 radius)
        {
            var time = clock.ElapsedMilliseconds*0.01f;
            
           // cube.transform.MoveVector(new Vector3((float)Math.Cos(angle),(float)Math.Sin(angle),0f));
        }

    }
}

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
            shadowFlag = true;
            sceneLight = new LightCamera(this);
            sceneLight.setLightData(new Vector4(0, 0f, 12f, 1), (Vector4)Color.White);
            Components = new List<GameComponent>();
            Components.Add(sceneLight);


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

            SurfaceComponent view = new SurfaceComponent(this, 50, 50, Color4.Black);
            view.SetTextureFile("ff");
            Components.Add(view);

             // Components.Add(lines);
             */
            cubeMini = new CubeComponent(this);
            cubeMini.transform.Scale = 2f;
            cubeMini.transform.Position = new Vector3(10,10,0);
            cubeMini.SetTextureFile("Moon.jpg");
            //Components.Add(cubeMini);

            GameModelComp cow = new GameModelComp(this, "cow.obj");
            cow.SetTextureFile("cow.jpg");
            cow.transform.Scale = 5f;
           // cow.transform.Position += new Vector3(30f, -15f, 0);
           // Components.Add(cow);

            GameModelComp cube = new GameModelComp(this,"crate.obj");
            cube.SetTextureFile("crate.jpg");
            cube.transform.Position += new Vector3(0f, 0f, -6f);
            cube.transform.Scale=10f;
            //Components.Add(cube);

            GameModelComp cube2 = new GameModelComp(this, "cow.obj");
            cube2.SetTextureFile("cow.jpg");
            cube2.transform.Position += new Vector3(0f, 0f, 0f);
            cube2.transform.Scale = 0.2f;
            Components.Add(cube2);

            SurfaceComponent view = new ShadowView(this, 0.5f, 0.5f, Color4.Black);
            view.transform.Position += new Vector3(-0.8f, 0.8f, 0f);       //              исправить трансформацию, не работает
           // Components.Add(view);

            Cells lines = new Cells(this);
            Components.Add(lines);

            TriangleComponent triangle = new TriangleComponent(this);
            Components.Add(triangle);

            StreamComponent stream = new StreamComponent(this);
           // Components.Add(stream);
        }

        public void Transformation(CubeComponent cube, Vector3 radius)
        {
            var time = clock.ElapsedMilliseconds*0.01f;
        }
        
    }
}

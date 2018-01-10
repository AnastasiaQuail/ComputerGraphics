using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Vector3 = SharpDX.Vector3;


namespace GameFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            MyGame game = new MyGame("Triangle",800, 800, new Vector3(0,5,28),false);
            //Pong game = new Pong("Pong", 800, 800, new Vector3(0, 0,200f));
            game.Run();
            game.Dispose();
        }
    }
}

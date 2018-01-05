using SharpDX.Direct3D11;
using SharpDX.DirectWrite;

namespace GameFramework
{
    class GameText:GameComponent
    {
        SharpDX.DirectWrite.Factory factory;
        TextFormat format;
        TextLayout textlayout;
        string text;

        public GameText()
        {
            factory = new Factory();
            format = new TextFormat(factory, "Segoe_UI", 16f);
            textlayout = new TextLayout(factory, "Text", format, 400.0f, 200.0f);
        }

        public override void Draw()
        {
            //DeviceContext2D;
            base.Draw();
        }
    }
}

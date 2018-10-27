using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Windows;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;

namespace GameFramework
{
    public class MyConsole
    {
        Game game;
        private SharpDX.Direct2D1.Factory d2dFactory;
        private int width;
        private int height;
        private RoundedRectangleGeometry rectangleGeometry;
        private Factory factory;
        private Texture2D backBuffer;
        private RenderTargetView renderView;
        private Surface surface;
        private RenderTarget d2dRenderTarget;
        private SolidColorBrush solidColorBrush;
        private MyTextWriter textWriter;
        private Vector2 consoleStart;
        private List<String> concoleText;

        public void Initialize(Game game)
        {
            this.game = game;

            d2dFactory = new SharpDX.Direct2D1.Factory();

            width = 500;
            height = 150;
            consoleStart = new Vector2(50, 50);

            rectangleGeometry = new RoundedRectangleGeometry(d2dFactory, new RoundedRectangle() { RadiusX = 5, RadiusY = 5, Rect = new RectangleF(10, 10, width, height) });

            // Ignore all windows events
            factory = game.swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(game.Form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(game.swapChain, 0);
            renderView = new RenderTargetView(game.device, backBuffer);

            surface = backBuffer.QueryInterface<Surface>();


            d2dRenderTarget = new RenderTarget(d2dFactory, surface,
                                                            new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));

            solidColorBrush = new SolidColorBrush(d2dRenderTarget, Color.White);

            textWriter = new MyTextWriter(d2dRenderTarget, consoleStart);
            concoleText = new List<string>();
        }
        public void Draw(Stopwatch stopwatch)
        {
                d2dRenderTarget.BeginDraw();
                solidColorBrush.Color = new Color4(Color3.Black,1.0f);
                d2dRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
                textWriter.WriteLine(concoleText);
                d2dRenderTarget.EndDraw();

                
        }
        public void WriteLine(string text)
        {
            concoleText.Add( (text+ "\n").ToString());
            if (concoleText.Count>7)
            {
                concoleText.RemoveAt(0);
            }
        }
        public void Dispose()
        {
            renderView.Dispose();
            backBuffer.Dispose();
            factory.Dispose();
        }
    }

}

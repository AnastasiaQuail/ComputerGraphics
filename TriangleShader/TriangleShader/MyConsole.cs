using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.RawInput;
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
        private InputDevice input;
        private bool flagConsoleOpen;
        private string rawInput;
        private TextBox tb;
        private LuaCompiller compiller;

        public void Initialize(Game game)
        {
            this.game = game;

            // allow input actions
            this.input = game.inputDevice;
            flagConsoleOpen = false;
            rawInput = "";
            game.Form.KeyPress += Input;

            // create Lua compiller element to run code from console
            compiller = new LuaCompiller();

            // create console window
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
            solidColorBrush.Color = new Color4(Color3.Black, 1.0f);
            d2dRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
            textWriter.WriteLine(concoleText);
            d2dRenderTarget.EndDraw();


        }
        public void WriteLine(string text)
        {
            concoleText.Add((text + "\n").ToString());
            if (concoleText.Count > 7)
            {
                concoleText.RemoveAt(0);
            }
        }
        public void Write(string text)
        {
            concoleText.RemoveAt(concoleText.Count-1);
            concoleText.Add((text).ToString());
            if (concoleText.Count > 7)
            {
                concoleText.RemoveAt(0);
            }
        }
        private void Remove()
        {
            if (rawInput.Length != 1)
            {
                rawInput = rawInput.Remove(rawInput.Length - 2);
            }
            else
            {
                rawInput = "";
            }
        }
        public void Dispose()
        {
            renderView.Dispose();
            backBuffer.Dispose();
            factory.Dispose();
        }
        public void Input(object sender, KeyPressEventArgs e)
        {
            string c;

            if (e.KeyChar == (char)Keys.Tab)  //was closed, but we open console for write with Tab
            {
                flagConsoleOpen = !flagConsoleOpen;//change flag
                rawInput = "";
                //concoleText.RemoveAt(concoleText.Count - 1);
                //concoleText.Add("_|");
                return;//clear previous input
            }
            if (flagConsoleOpen)
            {
                if (e.KeyChar != (char)Keys.Enter)
                {
                    if (e.KeyChar == (char)Keys.Back)
                    {
                        Remove();
                    }
                    else
                    {
                        c = e.KeyChar.ToString();
                        rawInput += c;
                    }
                    Write(rawInput);
                    return;
                }
                else
                {
                    Write(rawInput);
                    try
                    {
                        WriteLine(compiller.DoCode(rawInput));
                    }
                    catch( Exception exc)
                    {
                        WriteLine(exc.Message);
                        rawInput = "";
                        WriteLine("");
                        return;
                    }
                    rawInput = "";
                    WriteLine("");
                    return;
                }
            }
        }
    }
}
           

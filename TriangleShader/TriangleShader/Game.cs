using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.Diagnostics;
using SharpDX.Mathematics.Interop;

namespace GameFramework
{
    public class Game
    {
        RawBool IsFullScreen;
        Output SomeOut;
        public Device device;
        public SwapChain swapChain;
        public RenderForm Form;
        public DeviceContext context { get; set; }
        public RenderTargetView renderView;
        public Texture2D backBuffer;
        public bool IsActive;
        public Camera mycamera;
        public InputDevice inputDevice;
        public List<GameComponent> Components { get; set; }
        public Stopwatch clock {get;set; }
        private Texture2D zBuffer;
        public DepthStencilView depthView { get; set; }
        public Buffer lightBuffer;
        public LightData lightData;
       


        // Initialize
        public Game(string name, int fwidth, int fheigh, Vector3 position,bool flag) {
            IsActive = true;
            clock = new Stopwatch();
            inputDevice = new InputDevice(this);
            mycamera = new Camera(this,position,flag);
            Form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(fwidth, fheigh)
            };

            lightData = new LightData();
            InitializeDeviceResources();
        }
        private void InitializeDeviceResources()
        {
            //SwapChain Description
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(Form.ClientSize.Width, Form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            //Creating SwapChain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

            //Create backBuffer from SwapChain and create Render view
            backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
            renderView = new RenderTargetView(device, backBuffer);

            //Create context 
            context = device.ImmediateContext;


            zBuffer = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil, 
                CpuAccessFlags = CpuAccessFlags.None,
                MipLevels = 1,
                Format = Format.D32_Float,
                Height = Form.ClientSize.Height,
                Width = Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1,0),
                OptionFlags = ResourceOptionFlags.None
            });

            depthView = new DepthStencilView(device, zBuffer);

            CreateLightBuffer();
        }

        public void Dispose()
        {
            IsActive = false;

            renderView.Dispose();
            backBuffer.Dispose();
            context.ClearState();
            context.Flush();
            context.Dispose();
            zBuffer.Dispose();
            depthView.Dispose();
            device.Dispose();
            swapChain.Dispose();
            foreach (var component in Components)
            {
                component.Dispose();
            }
           
        }



        public virtual void Run()
        {
            if (!IsActive)
            { Dispose(); return; }
            else
            {
                clock.Start();

                if (inputDevice.IsKeyDown(System.Windows.Forms.Keys.F))
                {
                    swapChain.GetFullscreenState(out IsFullScreen, out SomeOut);
                    swapChain.SetFullscreenState(!IsFullScreen,SomeOut);
                }


                //Main loop
                RenderLoop.Run(Form, () =>
                {
                    if (!IsActive)
                      { Dispose(); Form.Close(); return; }

                    //Render view
                    mycamera.Render();

                    //update light
                    //UpdateLight();
					//context.UpdateSubresource(ref lightData, lightBuffer);

                    //Set background
                    context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                    context.ClearRenderTargetView(renderView, Color.Blue);

                //Update components
                foreach (var component in Components)
                    {

                        component.Update();

                    }

                //Drawing all components of the Game
                foreach (var component in Components)
                    {
                        component.Draw();
                    }

                //Prresent all
                swapChain.Present(0, PresentFlags.None);
                    if (inputDevice.IsKeyDown(System.Windows.Forms.Keys.Escape))
                    {
                        IsActive = false;
                    }
                });
                clock.Stop();
                Dispose();
            }
        }

        private void CreateLightBuffer()
        {
            //Create Constant buffer
            lightBuffer = new Buffer(device, Utilities.SizeOf<LightData>(),
               ResourceUsage.Default,
               BindFlags.ConstantBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None, 0);
        }

        public virtual void UpdateLight()
        {
            lightData.Position = new Vector4(0, 20, 0, 1);
        }


    }
}

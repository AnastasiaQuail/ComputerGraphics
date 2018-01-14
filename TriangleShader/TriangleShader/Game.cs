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
        private Texture2D shadowBuffer;
        public DepthStencilView shadowView;
        public bool shadowFlag;

        public DepthStencilView depthView { get; set; }
        public Buffer lightBuffer;
        public LightCamera sceneLight;
        public SamplerState shadowSampler;
        public ShaderResourceView shadowResourceView;


        // Initialize
        public Game(string name, int fwidth, int fheigh, Vector3 position,bool flag) {

            IsActive = true;
            shadowFlag = false;
            clock = new Stopwatch();
            inputDevice = new InputDevice(this);
            mycamera = new Camera(this,position,flag);
            sceneLight = new LightCamera(this);
            sceneLight.setLightData( Vector4.One, Color4.White);
            Form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(fwidth, fheigh)
            };
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
            CreateShadowBuffer();
        }

        public void Dispose()
        {
            IsActive = false;

            foreach (var component in Components)
            {
                component.Dispose();
            }

            context.ClearState();
            context.Flush();
            context.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            zBuffer.Dispose();
            depthView.Dispose();
            device.Dispose();
            swapChain.Dispose();

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

                    //  context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));

                    sceneLight.camera.Render();
                    //Render view
                    mycamera.Render();

                    //Set background
                    context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                    context.ClearRenderTargetView(renderView, Color.DarkSlateBlue);

                    //Clear shadow map
                    context.ClearDepthStencilView(shadowView, DepthStencilClearFlags.Depth, 1.0f, 0);

                    context.OutputMerger.ResetTargets();
                    context.OutputMerger.SetTargets(shadowView, renderView);

                    foreach (var component in Components)
                    {
                        if (component.lightFlag)
                        {
                            //Draw components to shadowMap
                            component.ShadowDraw();
                            CreateShadowSampler();
                            CreateShadowView();
                            component.Initialize(this, component.nameOfShader, true);
                        }
                        //Update components
                        component.Update();
                    }


                    context.OutputMerger.SetTargets(depthView, renderView);
                    context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));
                    context.ClearRenderTargetView(renderView, Color.DarkSlateBlue);

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
        private void CreateShadowBuffer()
        {
            shadowBuffer = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                MipLevels = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32_Typeless,
                Height = Form.ClientSize.Height,
                Width = Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None
            });

            shadowView = new DepthStencilView(device, shadowBuffer,new DepthStencilViewDescription
            {
                Format = Format.D32_Float,
                Dimension = DepthStencilViewDimension.Texture2D,
                Flags = DepthStencilViewFlags.None
            });
        }
        public void CreateShadowSampler()
        {
            shadowSampler = new SamplerState(device, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Less,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            });
        }
        public void CreateShadowView()
        {
            shadowResourceView = new ShaderResourceView(device, shadowBuffer,new ShaderResourceViewDescription
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            });
            context.GenerateMips(shadowResourceView);
        }

    }
}

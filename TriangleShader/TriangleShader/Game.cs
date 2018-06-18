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
        private Texture2D stencilBuffer;
        DepthStencilState depthState;
        public Texture2D shadowBuffer;
        public DepthStencilView shadowView;
        public bool shadowFlag;

        public DepthStencilView depthView { get; set; }
        public Buffer lightBuffer;
        public LightCamera sceneLight;
        public SamplerState shadowSampler;
        public ShaderResourceView shadowResourceView;
        StreamComponent stream;
        private DepthStencilStateDescription stencilStateDescription;
        private RasterizerState FrontRasterState;
        private CompilationResult shadowVolumeShaderBC;
        private VertexShader shadowVolumeShader;
        private RasterizerStateDescription rasterStateDesc;
        private RasterizerState backRasterState;
        private BlendState blendState, addBlendState;
        private RenderTargetBlendDescription blendStateDescription;
        private ShadowVolumeCube shadowCube;
        private DepthStencilView stencilView;
        public Render render;



        // Initialize
        public Game(string name, int fwidth, int fheigh, Vector3 position,bool flag) {

            IsActive = true;
            shadowFlag = false;
            clock = new Stopwatch();
            inputDevice = new InputDevice(this);
            mycamera = new Camera(this,position,flag);
            sceneLight = new LightCamera(this);
            sceneLight.setLightData( new Vector4(3.0f,3.0f,3.0f,1.0f), Color4.White);
            Form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(fwidth, fheigh)
            };
            InitializeDeviceResources();
            shadowCube = new ShadowVolumeCube(this);
            shadowCube.transform.Scale = 3.0f;
            stencilView = new DepthStencilView(device, stencilBuffer);
            render = new ForwardRenderer(this);
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
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out device, out swapChain);

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
                Format = Format.D32_Float_S8X24_UInt,
               // Format = Format.D24_UNorm_S8_UInt,
                Height = Form.ClientSize.Height,
                Width = Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1,0),
                OptionFlags = ResourceOptionFlags.None
            });

            stencilBuffer = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                MipLevels = 1,
                Format = Format.D32_Float_S8X24_UInt,
                Height = Form.ClientSize.Height,
                Width = Form.ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None
            });


            depthView = new DepthStencilView(device, zBuffer);

            CreateLightBuffer();
            CreateShadowBuffer();
            CreateShadowSampler();
            CreateShadowView();
            CreateStencilState();
            CreateFrontCulling();
            rasterStateDesc.CullMode = CullMode.Back;
            backRasterState = new RasterizerState(device, rasterStateDesc);
            CreateBlendState();

            
        }

        public void Dispose()
        {
            IsActive = false;

            foreach (var component in Components)
            {
                //component.Dispose();
            }
            try
                {
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
            catch (NullReferenceException e)
            {

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

                    context.ClearState();

                    context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));

                    sceneLight.camera.Render();
                    //Render view
                    mycamera.Render();

                    //Set backgroun
                    context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth|DepthStencilClearFlags.Stencil, 1.0f, 0);
                    context.ClearRenderTargetView(renderView, Color.DarkSlateBlue);

                    render.goRender();
                    //-------------Shadow map=-----------------//
                    //context.ClearDepthStencilView(shadowView, DepthStencilClearFlags.Depth, 1.0f, 0);

                    //context.OutputMerger.SetTargets(shadowView, renderView);

                    //foreach (var component in Components)
                    //{
                    //    ;
                    //    if (component.lightFlag)
                    //    {
                    //        //Draw components to shadowMap
                    //        component.ShadowDraw();
                    //    }
                    //}
                    //---------------------------------------------//

                    //------------Shadow volume----------------//
                    //context.OutputMerger.SetTargets(depthView, renderView);

                    //foreach (var component in Components)
                    //{
                    //    //Update components
                    //    component.Update();
                    //}

                    ////Drawing all components of the Game
                    //foreach (var component in Components)
                    //{
                    //    component.Draw();
                    //}
                    //shadowCube.Draw();
                    //RenderShadowVolume();

                    //Prresent all
                   // swapChain.Present(0, PresentFlags.None);
                    //---------------------------------------------//
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
        }

        public void CreateStencilState()
        {
            stencilStateDescription = new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
                IsStencilEnabled = true,
               // DepthWriteMask = DepthWriteMask.Zero,

                BackFace = new DepthStencilOperationDescription
                {
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    Comparison = Comparison.Always
                },
                FrontFace = new DepthStencilOperationDescription
                {
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    Comparison = Comparison.Always
                }
            };
            depthState = new DepthStencilState(device, stencilStateDescription);

        }
        private void RenderShadowVolume()
        {
            
            
            context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Stencil, 1.0f, 0);
            stencilStateDescription.IsStencilEnabled = false;
            stencilStateDescription.DepthWriteMask = DepthWriteMask.Zero;
            stencilStateDescription.StencilWriteMask = 0xff;

            depthState = new DepthStencilState(device, stencilStateDescription);
            context.OutputMerger.SetDepthStencilState(depthState);
          //  context.OutputMerger.BlendState = blendState;

            //foreach (GameComponent component in Components)
            //{
            //    component.Update();
            
            shadowCube.Update();
            context.VertexShader.Set(shadowVolumeShader);
            context.PixelShader.Set(null);
            shadowCube.DrawFromGB();
            //}
            //foreach (GameComponent component in Components)
            //{
            //    if (component!=sceneLight)
            //    component.Draw();
            //}
            //foreach (GameComponent component in Components)
            //{
            //    component.rasterState = FrontRasterState;
            //    component.Update();
            context.Rasterizer.State = FrontRasterState;
            context.VertexShader.Set(shadowVolumeShader);
            context.PixelShader.Set(null);
            shadowCube.DrawFromGB();
            //}
            //foreach (GameComponent component in Components)
            //{
            //    if (component != sceneLight)
            //        component.Draw();
            //}



            depthView = stencilView;

            stencilStateDescription.IsDepthEnabled = true;
            stencilStateDescription.IsStencilEnabled = true;
            stencilStateDescription.DepthComparison = Comparison.Always;
            stencilStateDescription.StencilReadMask = 0xff;
            stencilStateDescription.StencilWriteMask = 0x00;
            context.OutputMerger.DepthStencilReference = 0;
            stencilStateDescription.BackFace.Comparison = Comparison.Equal;
            depthState = new DepthStencilState(device, stencilStateDescription);

            depthState = new DepthStencilState(device, stencilStateDescription);
            context.OutputMerger.SetDepthStencilState(depthState);

            
            context.OutputMerger.BlendState = addBlendState;


            foreach (GameComponent component in Components)
            {
                component.rasterState = backRasterState;
                if (component.lightFlag)
                {
                    component.pixelShader = component.lightPixelShader;
                    component.vertexShader = component.lightVertexShader;
                }
                component.Update();
            }
            foreach (GameComponent component in Components)
            {
                if (component != sceneLight)
                    component.Draw();
            }
            shadowCube.rasterState = backRasterState;
            shadowCube.Draw();
            stencilStateDescription.IsStencilEnabled = false;
            stencilStateDescription.DepthWriteMask = DepthWriteMask.All;
            
            context.OutputMerger.SetDepthStencilState(depthState);
            context.OutputMerger.BlendState = blendState;
        }
        private void CreateFrontCulling()
        {
            rasterStateDesc = new RasterizerStateDescription
            {
                CullMode = CullMode.Front,
                FillMode = FillMode.Solid
            };
            FrontRasterState = new RasterizerState(device, rasterStateDesc);
        }
        private void CreateShadowVolumeShader()
        {
            try
            {
                shadowVolumeShaderBC = ShaderBytecode.CompileFromFile("Shaders/BCShadowVolume.fx", "VSMain", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            shadowVolumeShader = new VertexShader(device, shadowVolumeShaderBC);
        }
        private void CreateBlendState()
        {
            addBlendState = new BlendState(device, new BlendStateDescription());
            blendStateDescription = new RenderTargetBlendDescription(true, BlendOption.One, BlendOption.One, BlendOperation.Add, BlendOption.Zero, BlendOption.Zero, BlendOperation.Add, ColorWriteMaskFlags.All);
            addBlendState.Description.RenderTarget[0] = blendStateDescription;

            blendState = new BlendState(device, new BlendStateDescription());
            blendStateDescription = new RenderTargetBlendDescription
            {
                IsBlendEnabled = false
            };
            blendState.Description.RenderTarget[0] = blendStateDescription;

        }
       
    }
}

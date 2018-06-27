using System;
using System.Collections.Generic;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace GameFramework
{
    public struct LightDataSpot
    {
        public Vector4 Position;
        public Vector4 Color;
        public Matrix ViewProj;
        public float Range;
    }
    class LightVolume: GameModelComp
    {
        public ConstantDataDef data;
        ObjLoader objLoader;
        string fileName;
        private CompilationResult pixelShaderBCL;
        public PixelShader pixelShaderLight;
        public VertexShader vertexShaderLight;
        private CompilationResult vertexShaderBCL;
        private LightData light;
        private LightDataSpot spotLight;

        public LightVolume(Game game, string filename, float radius):base(game,filename)
        {
            data = new ConstantDataDef();
            nameOfShader = "Shaders/BCDefferedLight.fx";
            CreateVertexBufferLight(nameOfShader);
            CreatePixelBufferLight(nameOfShader);
            light = new LightData();
            spotLight = new LightDataSpot();
            spotLight.Range = radius;
        }
        public override void SetConstantData()
        {
            Matrix ProjectionM;
            //Update data
            game.mycamera.GetVPMatrix(out ViewProjMatrix);
            game.mycamera.GetProjectionMatrix(out ProjectionM);
            WorldMatrix = transform.GetWorldMatrix();
            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);

            //Set data
            InvertWorld = WorldMatrix;
            InvertWorld.Invert();

            data.WorldViewProj = WorldViewProjMatrix;
            data.World = WorldMatrix;
            data.InvertWorld = InvertWorld;
            data.ViewPos = new Vector4(transform.Position, 1);
            data.InverseProjectionView = Matrix.Invert(ProjectionM);

            context.UpdateSubresource(ref data, constantBuffer);

            if (lightFlag)
            {
                light = game.sceneLight.light;
                //spotLight.Color = light.Color;
                //spotLight.Position = light.Position;
                //spotLight.ViewProj = light.ViewProj;
                game.context.UpdateSubresource(ref light, game.lightBuffer);
            }
        }
        
        public void CreateVertexBufferLight(string filename)
        {
            //Create VertexBuffer  
            try
            {
                vertexShaderBCL = ShaderBytecode.CompileFromFile(filename, "LightingVS", "vs_5_0", ShaderFlags.PackMatrixRowMajor);
            }
            catch (ArgumentNullException e) { Console.WriteLine(e.Message); }
            vertexShaderLight = new VertexShader(device, vertexShaderBCL);
        }
        public void CreatePixelBufferLight(string filename)
        {
            //and PixelBuffer
            pixelShaderBCL = ShaderBytecode.CompileFromFile(filename, "LightingPS", "ps_5_0", ShaderFlags.PackMatrixRowMajor);
            pixelShaderLight = new PixelShader(device, pixelShaderBCL);
        }
    }
}

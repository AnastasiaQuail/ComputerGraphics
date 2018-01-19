using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    class ShadowView:SurfaceComponent
    {
        public ShadowView(Game gameObj, float width, float heigh, Vector4 color) : base(gameObj, width, heigh, color)
        {
            this.game = gameObj;
            transform = new Transform();
            this.width = width / 2f;
            this.heigh = heigh / 2f;
            z = 0;
            Initialize(game, "Shaders/BCTexture.fx", false);
        }
        public override void Initialize(Game game, string nameOfFile, bool IsLight)
        {
            base.Initialize(game, nameOfFile, IsLight);
            SetTextureFile(textureFile);
        }
        public override void CreateTextureShader()
        {
            textureView = new ShaderResourceView(device, game.shadowBuffer, new ShaderResourceViewDescription
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
        public override void Update()
        {
            //get transform from WorldViewProjMatrix
            ViewProjMatrix = Matrix.Identity;
            WorldMatrix = transform.GetWorldMatrix();

            Matrix.Multiply(ref WorldMatrix, ref ViewProjMatrix, out WorldViewProjMatrix);
            SetConstantData();
        }
    }
}

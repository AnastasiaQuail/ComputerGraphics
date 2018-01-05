using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;

namespace GameFramework
{
    class TextureLoader
    {
        Game game;
        public ImagingFactory Factory { protected set; get; }


        public TextureLoader(Game game)
        {
            this.game = game;

            Factory = new ImagingFactory();
        }


        public Texture2D LoadTextureFromFile(string fileName)
        {
            Texture2D tex = null;

            var decoder = new BitmapDecoder(Factory, fileName, DecodeOptions.CacheOnDemand);
            var frame = decoder.GetFrame(0);
            //var pixFormat = frame.PixelFormat;

            //var queryReader = frame.MetadataQueryReader;

            FormatConverter converter = new FormatConverter(Factory);
            converter.Initialize(frame, PixelFormat.Format32bppPRGBA);

            var width = converter.Size.Width;
            var height = converter.Size.Height;

            int stride = width * 4;

            using (var buffer = new SharpDX.DataBuffer(stride * height))
            {
                converter.CopyPixels(stride, buffer.DataPointer, buffer.Size);

                tex = new Texture2D(game.device, new Texture2DDescription
                {
                    Width = width,
                    Height = height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    Usage = ResourceUsage.Default,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.R8G8B8A8_UNorm,
                    MipLevels = 0,
                    OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                    SampleDescription = new SampleDescription(1, 0)
                });

                game.context.UpdateSubresource(new SharpDX.DataBox(buffer.DataPointer, stride, height), tex); 
            }


            return tex;

        }
    }
}

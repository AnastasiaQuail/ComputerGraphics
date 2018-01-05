using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    class SurfacePlane:SurfaceComponent
    {
        public SurfacePlane(Game gameObj, float width, float heigh, Vector4 color) : base(gameObj, width, heigh, color)
        {
            textureFile = "Texture.jpg";
        }
    }
}

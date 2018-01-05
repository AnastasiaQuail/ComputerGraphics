using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public struct ConstantData
    {
        public Matrix WorldViewProj;
        public Matrix World;
        public Matrix InvertWorld;
        public Vector4 ViewPos;

    }
}

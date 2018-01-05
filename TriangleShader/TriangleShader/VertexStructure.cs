using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public static class VertexStructures
    {
        public struct VertexPositionNormalTex
        {
            public Vector4 Position;
            public Vector4 Normal;
            public Vector4 Tex;
        }
        public static Vector4[] ToArray(List<VertexPositionNormalTex> list)
        {
            Vector4[] array = new Vector4[list.Count * 2];

            for (int i = 0; i < list.Count; i++)
            {
                array[i * 2] = list[i].Position;
                array[i * 2 + 1] = list[i].Tex;
            }

            return array;
        }
    }
}

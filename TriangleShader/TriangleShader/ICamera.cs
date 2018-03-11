using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    abstract class ICamera
    {
        private float pitch { get; set; }
        private float yaw { get; set; }
        private float roll { get; set; }
        private Vector3 Position;
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjMatrix { get; set; }
        public Matrix ViewProj { get; set; }
        public Game game;
        public Matrix rotationMatrix;
        public Vector3 lookAt { get; set; }
        private bool flag;

    }
}

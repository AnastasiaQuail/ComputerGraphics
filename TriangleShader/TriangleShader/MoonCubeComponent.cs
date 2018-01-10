using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    class MoonCubeComponent:CubeComponent
    {
        CubeComponent parent;
        public MoonCubeComponent(Game gameObj,CubeComponent parent):base(gameObj)
        {
            this.parent = parent;
            textureFile = "Texture.jpg";
            transform.RotationZ = -30;
            transform.RotationX = -30;
        }
        public override void Transformation()
        {
             Matrix trans = Matrix.Scaling(0.5f)  * Matrix.Translation(new Vector3(3f, 0f, 0f)) * Matrix.RotationZ(transform.Rotate());
            
             WorldMatrix =trans * parent.WorldMatrix;
        }
    }
}

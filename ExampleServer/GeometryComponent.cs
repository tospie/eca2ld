using ECABaseModel;
using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public static class GeometryComponent
    {
        public static void Register()
        {
            ComponentPrototype geometry = new ComponentPrototype("geometry");
            geometry.AddAttribute<byte[]>("binaryMesh");
            geometry.AddAttribute<string>("filetype");
            ComponentRegistry.Instance.Register(geometry);
        }
    }
}

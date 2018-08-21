using ECABaseModel;
using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public static class SpatialEntityComponent
    {

        public struct position
        {
            public double x;
            public double y;
            public double z;
        };

        public struct orientation
        {
            public double x;
            public double y;
            public double z;
            public double w;
        };

        public static void RegisterComponents()
        {
            ComponentPrototype spatial = new ComponentPrototype("spatial");
            spatial.AddAttribute<position>("position");
            spatial.AddAttribute<orientation>("orientation");
            spatial.AddAttribute<string>("id");
            ComponentRegistry.Instance.Register(spatial);
        }
    }
}

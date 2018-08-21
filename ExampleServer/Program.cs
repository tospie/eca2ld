/*
Copyright 2018 T.Spieldenner, DFKI GmbH

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using ECABaseModel;
using ECA2LD.Datapoints;
using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECABaseModel.Prototypes;

namespace ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SpatialEntityComponent.RegisterComponents();
            GeometryComponent.Register();

            Entity s = new Entity();
            s["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 0, y = 0, z = 0 });
            s["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            s["spatial"]["id"].Set("AkkuSchrauber");

            // Define a component with custom name. This name will be used to access the component on any entity.
            ComponentPrototype p = new ComponentPrototype("component");

            // Add a set of attributes. The names are used to access the attributes on instatiated components
            p.AddAttribute<float>("numeric");
            p.AddAttribute<string>("string");
            p.AddAttribute<bool>("boolean");

            // Once the protoype and its set of attributes is defined, register it to the global Component Registry
            ComponentRegistry.Instance.Register(p);

            // We are now all set to use our defined component on entities. For this, first, we create a new empty entity.
            Entity e = new Entity();

            // Components are automatically instatiated on first access. So we can just set the values on our entity as follow:
            e["component"]["numeric"].Set(3.14f);
            e["component"]["string"].Set("Hello World");
            e["component"]["boolean"].Set(true);

            s["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 4.12, y = 0.21, z = 0 });
            s["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            s["spatial"]["id"].Set("shop-floor-constTable-01AWX");

            s["geometry"]["binaryMesh"].Set(new byte[1] { 0 });
            s["geometry"]["filetype"].Set("3dfile");

            // Last, we expose our entity on an HTTP datapoint as Linked Data object. The ECA2LD lib will take care of building the correct
            // RDF graph, and creating and wiring datapoints for the linked component and attribute instances.
            var eDP = new EntityDatapoint(e, "http://localhost:12345/entities/e/");
            var sDP = new EntityDatapoint(s, "http://localhost:12345/entities/tisch/");

            // Our entity is now ready and set to be added to the world. The attributes could have been set as above afterwards as well.
            // Then events would have informed other parts of the program that our entity was changed.
            CEC.Instance.Add(e);
            CEC.Instance.Add(s);

            // This concludes the example. In the future, support to add datapoints on the Entity Collection should be implemented. This
            // would automatize the process of creating datapoints for each entity manually.
            Console.ReadKey();
        }
    }
}

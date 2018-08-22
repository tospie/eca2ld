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
            string host = "localhost";
            string port = "12345";

            if (args.Length > 0)
                if (args.Length != 2)
                    Console.WriteLine("Please supply exactly two arguments for host configuration: <hostname> <port>. Using default values <localhost> <12345>.");
                else
                {
                    host = args[0];
                    port = args[1];
                    try
                    {
                        var uri = new Uri("http://" + host + ":" + port + "/entities/");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Supplied arguments {0}, {1} do not form a valid host specifaction. Using default vaules <localhost> <12345>.", args[0], args[1]);
                        host = "localhost";
                        port = "12345";
                    }
                }

            string basePath = "http://" + host + ":" + port + "/entities/";

            Console.WriteLine("Entities are hosted at " + basePath);
            SpatialEntityComponent.RegisterComponents();
            GeometryComponent.Register();
            RobotComponent.Register();

            var tisch = buildTisch();
            var schrauber = buildSchrauber();
            var worker = birthWorker();
            var robot = assembleRobot(basePath);

            // Last, we expose our entity on an HTTP datapoint as Linked Data object. The ECA2LD lib will take care of building the correct
            // RDF graph, and creating and wiring datapoints for the linked component and attribute instances.
            var tischDP = new EntityDatapoint(tisch, basePath + "tisch/");
            var schraubDP = new EntityDatapoint(schrauber, basePath + "schrauber/");
            var workerDP = new EntityDatapoint(worker, basePath + "worker/");
            var robotDP = new EntityDatapoint(robot, basePath + "robot/");

            // Our entity is now ready and set to be added to the world. The attributes could have been set as above afterwards as well.
            // Then events would have informed other parts of the program that our entity was changed.
            CEC.Instance.Add(tisch);
            CEC.Instance.Add(schrauber);
            CEC.Instance.Add(worker);

            // This concludes the example. In the future, support to add datapoints on the Entity Collection should be implemented. This
            // would automatize the process of creating datapoints for each entity manually.
            Console.WriteLine("\n\n Press Key to shutdown");
            Console.ReadKey();
        }

        private static Entity buildTisch()
        {
            var s = new Entity();
            s["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 4.12, y = 0.21, z = 0 });
            s["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            s["spatial"]["id"].Set("shop-floor-constTable-01AWX");

            s["geometry"]["binaryMesh"].Set(new byte[1] { 0 });
            s["geometry"]["filetype"].Set("3dfile");
            return s;
        }

        private static Entity buildSchrauber()
        {
            var s = new Entity();
            s["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 0.7, y = 0.7, z = 0.14 });
            s["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            s["spatial"]["id"].Set("schrowbmast0r-scr3w2000");

            s["geometry"]["binaryMesh"].Set(new byte[1] { 0 });
            s["geometry"]["filetype"].Set("3dfile");
            return s;
        }

        private static Entity birthWorker()
        {
            var s = new Entity();
            s["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 0, y = 0, z = 0 });
            s["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            s["spatial"]["id"].Set("worker");

            return s;
        }

        private static Entity assembleRobot(string basePath)
        {
            var r = new Entity();

            var arm = new Entity();
            var platform = new Entity();
            arm["spatial"]["id"].Set("RobotArm");
            platform["spatial"]["id"].Set("Platform");

            r["robot"]["arm"].Set(arm);
            r["robot"]["platform"].Set(platform);
            r["spatial"]["position"].Set(new SpatialEntityComponent.position { x = 0, y = 0, z = 0 });
            r["spatial"]["orientation"].Set(new SpatialEntityComponent.orientation { x = 0, y = 0, z = 1, w = 0 });
            r["spatial"]["id"].Set("mobiPick");
            return r;
        }
    }
}

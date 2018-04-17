using ECABaseModel;
using ECA2LD.Datapoints;
using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Entity testEntity = new Entity();
            var eDP = new EntityDatapoint(testEntity, "http://localhost:12345/entities/e/");
            Console.ReadKey();
        }
    }
}

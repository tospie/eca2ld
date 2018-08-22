using ECABaseModel;
using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public static class RobotComponent
    {
        public static void Register()
        {
            ComponentPrototype robot = new ComponentPrototype("robot");
            robot.AddAttribute<Entity>("arm");
            robot.AddAttribute<Entity>("platform");
            ComponentRegistry.Instance.Register(robot);
        }
    }
}

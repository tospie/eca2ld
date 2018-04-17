using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDPDatapoints;
using ECABaseModel.Prototypes;
using ECA2LD.ldp_ttl;

namespace ECA2LD.Datapoints
{
    class ComponentPrototypeDatapoint : Resource
    {
        ComponentPrototypeGraph graph;

        public ComponentPrototypeDatapoint(ComponentPrototype p, string route) : base(route)
        {
            graph = new ComponentPrototypeGraph(new Uri(route), p);
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {
            string graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }

        protected override void onPost(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void onPut(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

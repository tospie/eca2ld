using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECABaseModel;
using ECA2LD.ldp_ttl;
using ECABaseModel.Prototypes;
using LDPDatapoints.Resources;
using LDPDatapoints;
using System.Net;

namespace ECA2LD.Datapoints
{
    class ComponentDatapoint : Resource
    {
        private ComponentLDPGraph graph;

        public ComponentDatapoint(Component component, string route) : base(route)
        {
            graph = new ComponentLDPGraph(new Uri(route), component);
            try
            {
                new ComponentPrototypeDatapoint(component.Definition, new Uri(route).getPrototypeBaseUri() + component.Name + "/");
            }
            catch (HttpListenerException) { }
            foreach (AttributePrototype a in component.Definition.AttributeDefinitions)
            {
                new AttributeDatapoint(component[a.Name], route.TrimEnd('/') + "/" + a.Name);
            }
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

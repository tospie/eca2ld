using ECA2LD.ldp_ttl;
using ECABaseModel;
using LDPDatapoints;
using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECA2LD.Datapoints
{
    public class EntityDatapoint : Resource
    {
        EntityLDPGraph graph;

        public EntityDatapoint(Entity value, string route) : base(route)
        {
            graph = new EntityLDPGraph(new Uri(route), value);
            foreach(Component c in value.Components)
            {
                ComponentDatapoint cd = new ComponentDatapoint(c, route.TrimEnd('/') + "/" + c.Name + "/");
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

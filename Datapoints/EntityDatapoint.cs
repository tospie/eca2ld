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

    internal static class EntityDatapointManager
    {
        private static Dictionary<Guid, EntityDatapoint> datapoints = new Dictionary<Guid, EntityDatapoint>();

        public static void SetDatapoint(this Entity entity, EntityDatapoint datapoint)
        {
            datapoints.Add(entity.Guid, datapoint);
        }

        public static EntityDatapoint GetDatapoint(this Entity entity)
        {
            return datapoints[entity.Guid];
        }

        public static bool HasDatapoint(this Entity entity)
        {
            return datapoints.ContainsKey(entity.Guid);
        }
    }

    public class EntityDatapoint : Resource
    {
        EntityLDPGraph graph;

        public EntityDatapoint(Entity value, string route) : base(route)
        {
            graph = new EntityLDPGraph(new Uri(route), value);
            value.SetDatapoint(this);
            foreach (Component c in value.Components)
            {
                ComponentDatapoint cd = new ComponentDatapoint(c, route.TrimEnd('/') + "/" + c.Name + "/");
            }
            value.CreatedComponent += (o, e) =>
            {
                new ComponentDatapoint(e.Component, route.TrimEnd('/') + "/" + e.Component.Name + "/");
            };
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

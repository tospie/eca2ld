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
using VDS.RDF;

namespace ECA2LD.Datapoints
{
    public class EntityCollectionDatapoint : CollectionResource<EntityCollection, Entity>
    {
        EntityCollectionLDPGraph graph;
        string route;

        public EntityCollectionDatapoint(EntityCollection collection, string route) : base(collection, route)
        {
            graph = new EntityCollectionLDPGraph(new Uri(route), collection);
            this.route = route;

            lock (collection)
            {
                foreach (Entity e in collection)
                {
                    createDatapointOnEntity(e);
                }
            }

            collection.AddedEntity += (o, e) => createDatapointOnEntity(e.Entity);
        }

        private void createDatapointOnEntity(Entity e)
        {
            if (!e.HasDatapoint())
            {
                var entityDatapoint = new EntityDatapoint(e, route.TrimEnd('/') + "/" + e.Guid + "/");
            }
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {
            string graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }
    }
}

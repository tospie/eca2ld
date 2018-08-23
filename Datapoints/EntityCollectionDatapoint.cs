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
using System.IO;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace ECA2LD.Datapoints
{
    public class EntityCollectionDatapoint : CollectionResource<EntityCollection, Entity>
    {
        EntityCollectionLDPGraph graph;
        string route;
        TurtleParser turtleParser = new TurtleParser();

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

        protected override void onPost(object sender, HttpEventArgs e)
        {
            e.response.StatusCode = 201;
            string returnMessage = "Object created";
            if (e.request.ContentType.Contains("text/turtle"))
            {
                try
                {
                    var receivedGraph = parseTurtle(new Graph(), e.request.InputStream);
                    graph.Extend(receivedGraph);
                }
                catch (Exception ex)
                {
                    e.response.StatusCode = 400;
                    returnMessage = "Could not process the supplied turtle code. Exception: " + ex.Message;

                }
            }

            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(returnMessage), 0, returnMessage.Length);
            e.response.OutputStream.Close();
        }

        private Graph parseTurtle(Graph g, Stream inputStream)
        {
            using (Stream input = inputStream)
            {
                using (StreamReader reader = new StreamReader(input, Encoding.UTF8))
                {
                    turtleParser.Load(g, reader);
                }
            }
            return g;
        }
    }
}

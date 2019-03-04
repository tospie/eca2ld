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
using VDS.RDF;
using VDS.RDF.Query;

namespace ECA2LD.Datapoints
{

    internal static class ComponentDatapointManager
    {
        private static Dictionary<Guid, ComponentDatapoint> datapoints = new Dictionary<Guid, ComponentDatapoint>();

        public static void SetDatapoint(this Component component, ComponentDatapoint datapoint)
        {
            lock(datapoints)
                datapoints.Add(component.Guid, datapoint);
        }

        public static ComponentDatapoint GetDatapoint(this Component component)
        {
            lock(datapoints)
                return datapoints[component.Guid];
        }

        public static bool HasDatapoint(this Component component)
        {
            lock(datapoints)
                return datapoints.ContainsKey(component.Guid);
        }
    }

    /// <summary>
    /// Creates an ECA2LD HTTP / RDF endpoint around an <seealso cref="ECABaseModel.Component"/>
    /// </summary>
    class ComponentDatapoint : Resource
    {
        internal ComponentLDPGraph graph;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component"><seealso cref="ECABaseModel.Component"/> that is to be represented by the RDF endpoint</param>
        /// <param name="uri">Endpoint listener URI</param>
        public ComponentDatapoint(Component component, string uri) : base(uri)
        {
            graph = new ComponentLDPGraph(new Uri(uri), component);
            component.SetDatapoint(this);
            try
            {
                lock (ComponentPrototypeManager.RegisteredPrototypes)
                    if (!ComponentPrototypeManager.RegisteredPrototypes.ContainsKey(component.Prototype.Name))
                    {
                        ComponentPrototypeManager.RegisteredPrototypes.Add(component.Prototype.Name,
                               new ComponentPrototypeDatapoint(component.Prototype, new Uri(uri).getPrototypeBaseUri() + component.Name + "/"));
                    }

            }
            catch (HttpListenerException) { }
            foreach (AttributePrototype a in component.Prototype.AttributePrototypes)
            {
                new AttributeDatapoint(component[a.Name], uri.TrimEnd('/') + "/" + a.Name);
            }
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {
            string graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }

        protected override void onOptions(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void onPost(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void onPut(object sender, HttpEventArgs e)
        {
            PutHandler.handleRequest(e, Route, processReceivedGraph);
        }

        private void processReceivedGraph(Graph g)
        {
            SparqlResultSet results = SparqlExecutor.PerformQuery("SELECT DISTINCT ?s ?o WHERE { ?s dct:hasPart ?o }", g);
            foreach (SparqlResult r in results)
            {
                if (!r.Value("s").ToString().Equals(Route))
                    continue;
                graph.AddAttributeTriple(r.Value("o").ToString());
            }
        }
    }
}

﻿/*
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
using ECABaseModel.Events;
using LDPDatapoints;
using LDPDatapoints.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace ECA2LD.Datapoints
{

    internal static class EntityCollectionDatapointManager
    {
        private static Dictionary<Guid, EntityCollectionDatapoint> datapoints = new Dictionary<Guid, EntityCollectionDatapoint>();

        public static void SetDatapoint(this EntityCollection entityCollection, EntityCollectionDatapoint datapoint)
        {
            datapoints.Add(entityCollection.Guid, datapoint);
        }

        public static EntityCollectionDatapoint GetDatapoint(this EntityCollection entityCollection)
        {
            return datapoints[entityCollection.Guid];
        }

        public static bool HasDatapoint(this EntityCollection entityCollection)
        {
            return datapoints.ContainsKey(entityCollection.Guid);
        }
    }

    public class EntityCollectionDatapoint : CollectionResource<EntityCollection, Entity>
    {
        EntityCollectionLDPGraph graph;
        BasicLDPGraph completeGraph;

        string route;
        TurtleParser turtleParser = new TurtleParser();

        public EntityCollectionDatapoint(EntityCollection collection, string route) : base(collection, route)
        {
            graph = new EntityCollectionLDPGraph(new Uri(route), collection);
            completeGraph = new BasicLDPGraph(new Uri(route + "rdf/"));
            this.route = route;

            collection.SetDatapoint(this);
            lock (collection)
            {
                foreach (Entity e in collection)
                {
                    createDatapointOnEntity(e);
                    addEntityToCompleteGraph(e);
                    completeGraph.RDFGraph.Merge(graph.RDFGraph);
                    e.ChangedAttribute += new EventHandler<ChangedAttributeEventArgs>(updateCompleteGraph);
                }
            }

            collection.AddedEntity += (o, e) =>
            {
                createDatapointOnEntity(e.Entity);
                addEntityToCompleteGraph(e.Entity);
                completeGraph.RDFGraph.Merge(graph.RDFGraph);
                e.Entity.ChangedAttribute += new EventHandler<ChangedAttributeEventArgs>(updateCompleteGraph);
            };
        }

        private void updateCompleteGraph(object sender, ChangedAttributeEventArgs e)
        {
            string attributeUri = e.Component[e.AttributeName].GetDatapoint().Route;
            completeGraph.RDFGraph.Retract(new Triple(
                completeGraph.RDFGraph.CreateUriNode(new Uri(attributeUri)),
                completeGraph.RDFGraph.CreateUriNode("rdf:value"),
                completeGraph.RDFGraph.CreateLiteralNode(e.OldValue.ToString(), new Uri("xsd:attributeValue"))));
            completeGraph.RDFGraph.Assert(new Triple(
                completeGraph.RDFGraph.CreateUriNode(new Uri(attributeUri)),
                completeGraph.RDFGraph.CreateUriNode("rdf:value"),
                completeGraph.RDFGraph.CreateLiteralNode(e.NewValue.ToString(), new Uri("xsd:attributeValue"))));
        }

        private void createDatapointOnEntity(Entity e)
        {
            if (!e.HasDatapoint())
            {
                var entityDatapoint = new EntityDatapoint(e, route.TrimEnd('/') + "/" + e.Guid + "/");
            }
        }

        private void addEntityToCompleteGraph(Entity e)
        {
            completeGraph.RDFGraph.Merge(e.GetDatapoint().graph.GetMergedGraph());
        }

        public override void onGet(object sender, HttpEventArgs e)
        {
            if (e.request.QueryString.Get("query") != null)
                onSparql(e);

            string graphAsTTL;
            string entailment = e.request.QueryString.Get("entailment");
            if (entailment != null && entailment.Equals("complete"))
                graphAsTTL = completeGraph.GetTTL();
            else
                graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }

        protected void onSparql(HttpEventArgs e)
        {
            string query = e.request.QueryString.Get("query");
            StreamWriter w = new StreamWriter(e.response.OutputStream);
            try
            {
                var result = SparqlExecutor.PerformQuery(query, completeGraph.RDFGraph);
                e.response.ContentType = "application/json";
                e.response.StatusCode = 200;
                w.Write(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                string errorMessage = "Provided query produced an error: " + ex.Message;
                e.response.StatusCode = 400;
                w.Write(errorMessage);
            }
            w.Flush();
            e.response.OutputStream.Close();
        }


        /// <summary>
        /// POST Endpoint accepts a valid turtle snippet that specifies containment of entities in a remote endpoint,
        /// comparable to the RDF-Graph in ttl that the Entity Collection Datapoint produces upon a GET request.
        /// Example for a valid Turtle-RDF:
        ///
        /// @prefix ldp: <http://www.w3.org/ns/ldp#>.  // Prefix needs to be included to allow parsing
        ///
        /// <http://host:port/base-path/> ldp:contains <http://host:port/base-path/entity-path>
        ///
        /// The above snippet will lead to the EntityCollectionDatapoint contain the triples
        /// <collectionDataPoint> rdf:seeAlso <http://host:port/base-path/> .
        /// <collectionDataPoint> foaf:knows <http://host:port/base-path/entity-path> .
        ///
        /// These triples express that information about additional entities can be retrieved from the remote host.
        /// The local EntityCollection is aware of more entities, which are hosted on remote hosts, but of which data is
        /// not present in the local dataset.
        /// </summary>
        public override void onPost(object sender, HttpEventArgs e)
        {
            e.response.StatusCode = 201;
            string returnMessage = "Object created";
            if (e.request.ContentType.Contains("text/turtle"))
            {
                try
                {
                    var receivedGraph = parseTurtle(new Graph(), e.request.InputStream);
                    processGraph(receivedGraph);
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

        private void processGraph(Graph g)
        {
            SparqlResultSet results = SparqlExecutor.PerformQuery("SELECT ?s ?o WHERE { ?s ldp:contains ?o }", g);

            foreach (var r in results.Results)
            {
                graph.AddExternalContainer(r.Value("s").ToString());
                graph.AddExternalEntity(r.Value("o").ToString());
            }
        }
    }
}

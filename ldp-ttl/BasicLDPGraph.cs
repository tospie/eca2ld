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
using VDS.RDF;
using VDS.RDF.Writing;

namespace ECA2LD.ldp_ttl
{
    public class BasicLDPGraph
    {
        public Graph RDFGraph { get; private set; }

        /// <summary>
        /// RDF PREDICATE NODES
        /// </summary>

        protected IUriNode RDF_TYPE;
        protected IUriNode DCT_IDENTIFIER;
        protected IUriNode LDP_HASMEMBERRELATION;
        protected IUriNode DCT_HAS_PART;
        protected IUriNode DCT_IS_PART_OF;
        protected IUriNode RDFS_IS_DEFINED_BY;
        protected IUriNode RDF_VALUE;

        /// <summary>
        /// RDF OBJECT NODES
        /// </summary>

        protected IUriNode LDP_BASIC_CONTAINER;
        protected IUriNode LDP_RDF_RESOURCE;
        protected IUriNode XSD_STRING;

        // Uri of Datapoint and Uri Node of the ECA-Object that is described by the graph
        protected string dp_uri;
        protected IUriNode un;
        protected CompressingTurtleWriter writer;

        public BasicLDPGraph(Uri u)
        {
            writer = new CompressingTurtleWriter(VDS.RDF.Parsing.TurtleSyntax.W3C);
            RDFGraph = new Graph();
            dp_uri = u.ToString();
            un = RDFGraph.CreateUriNode(u);
            addNamespaces();
            addPredicateNodes();
            addObjectNodes();
        }

        public string GetTTL()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            writer.Save(RDFGraph, sw);
            return sw.ToString();
        }

        protected void addNamespaces()
        {
            RDFGraph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            RDFGraph.NamespaceMap.AddNamespace("ldp", new Uri("http://www.w3.org/ns/ldp#"));
            RDFGraph.NamespaceMap.AddNamespace("dct", new Uri("http://purl.org/dc/terms/"));
            RDFGraph.NamespaceMap.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
        }

        protected void addPredicateNodes()
        {
            RDF_TYPE = RDFGraph.CreateUriNode("rdf:type");
            DCT_IDENTIFIER = RDFGraph.CreateUriNode("dct:identifer");
            LDP_HASMEMBERRELATION = RDFGraph.CreateUriNode("ldp:hasMemberRelation");
            DCT_HAS_PART = RDFGraph.CreateUriNode("dct:hasPart");
            DCT_IDENTIFIER = RDFGraph.CreateUriNode("dct:identifier");
            DCT_IS_PART_OF = RDFGraph.CreateUriNode("dct:isPartOf");
            RDFS_IS_DEFINED_BY = RDFGraph.CreateUriNode("rdfs:isDefinedBy");
            RDF_VALUE = RDFGraph.CreateUriNode("rdf:value");
        }

        protected void addObjectNodes()
        {
            LDP_BASIC_CONTAINER = RDFGraph.CreateUriNode("ldp:BasicContainer");
            LDP_RDF_RESOURCE = RDFGraph.CreateUriNode("ldp:RDFResource");
            XSD_STRING = RDFGraph.CreateUriNode("xsd:string");
        }

        protected virtual void BuildRDFGraph() { }
    }
}

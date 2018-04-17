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

using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace ECA2LD.ldp_ttl
{
    class ComponentPrototypeGraph : BasicLDPGraph
    {
        private ReadOnlyComponentPrototype componentPrototype;
        private Graph attributesGraph;
        private IUriNode ECA_COMPONENT;

        public ComponentPrototypeGraph(Uri u, ReadOnlyComponentPrototype p) : base(u)
        {
            componentPrototype = p;
            RDFGraph.NamespaceMap.AddNamespace("eca", new Uri("http://www.dfki.de/eca#"));
            ECA_COMPONENT = RDFGraph.CreateUriNode("eca:component");
            attributesGraph = new Graph();
            attributesGraph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            attributesGraph.NamespaceMap.AddNamespace("ldp", new Uri("http://www.w3.org/ns/ldp#"));
            attributesGraph.NamespaceMap.AddNamespace("dct", new Uri("http://purl.org/dc/terms/"));
            attributesGraph.NamespaceMap.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            attributesGraph.NamespaceMap.AddNamespace("eca", new Uri("http://www.dfki.de/eca#"));
            createAttributePrototypesGraph();
            BuildRDFGraph();
            RDFGraph.Merge(attributesGraph, false);
        }

        private void createAttributePrototypesGraph()
        {
            foreach (AttributePrototype a in componentPrototype.AttributePrototypes)
            {
                createAttributePrototypeNode(a);
            }
        }

        private void createAttributePrototypeNode(AttributePrototype a)
        {
            ILiteralNode n_a = attributesGraph.CreateLiteralNode(a.Name);
            IUriNode u_a = createAttributeUriNode(a, attributesGraph);
            attributesGraph.Assert(new Triple(
                u_a,
                attributesGraph.CreateUriNode("dct:Identifier"),
                n_a));

            attributesGraph.Assert(new Triple(
                u_a,
                attributesGraph.CreateUriNode("rdf:Type"),
                attributesGraph.CreateUriNode("eca:attribute")));

            attributesGraph.Assert(new Triple(
                u_a,
                attributesGraph.CreateUriNode("rdf:Type"),
                attributesGraph.CreateLiteralNode(a.Type.ToString(), "xsd:type")));

            attributesGraph.Assert(new Triple(
                u_a,
                attributesGraph.CreateUriNode("rdf:Value"),
                attributesGraph.CreateBlankNode(a.Name)));
        }

        private IUriNode createAttributeUriNode(AttributePrototype a, Graph g)
        {
            string attributeUri = dp_uri.TrimEnd('/') + "/" + a.Name;
            return g.CreateUriNode(new Uri(attributeUri));
        }

        protected override void BuildRDFGraph()
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, ECA_COMPONENT));
            foreach (AttributePrototype a in componentPrototype.AttributePrototypes)
            {
                IUriNode u_a = createAttributeUriNode(a, RDFGraph);
                RDFGraph.Assert(un, DCT_HAS_PART, u_a);
            }
        }
    }
}

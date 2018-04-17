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
        private ComponentPrototype componentPrototype;
        private Graph attributesGraph;
        private IUriNode ECA_COMPONENT;

        public ComponentPrototypeGraph(Uri u, ComponentPrototype p) : base(u)
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
            createAttributeDefinitionsGraph();
            BuildRDFGraph();
            RDFGraph.Merge(attributesGraph, false);
        }

        private void createAttributeDefinitionsGraph()
        {
            foreach (AttributePrototype a in componentPrototype.AttributeDefinitions)
            {
                createAttributeDefinitionNode(a);
            }
        }

        private void createAttributeDefinitionNode(AttributePrototype a)
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
            foreach (AttributePrototype a in componentPrototype.AttributeDefinitions)
            {
                IUriNode u_a = createAttributeUriNode(a, RDFGraph);
                RDFGraph.Assert(un, DCT_HAS_PART, u_a);
            }
        }
    }
}

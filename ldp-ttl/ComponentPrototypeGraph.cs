using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace SIXPrimeLDPlugin.ldp_ttl
{
    class ComponentPrototypeGraph : BasicLDPGraph
    {
        private ComponentDefinition componentPrototype;
        private Graph attributesGraph;
        private IUriNode ECA_COMPONENT;

        public ComponentPrototypeGraph(Uri u, ComponentDefinition d) : base(u)
        {
            componentPrototype = d;
            RDFGraph.NamespaceMap.AddNamespace("eca", new Uri("http://www.dfki.de/eca#"));
            ECA_COMPONENT = RDFGraph.CreateUriNode("eca:component");
            attributesGraph = new Graph();
            createAttributeDefinitionsGraph();
            BuildRDFGraph();
            RDFGraph.Merge(attributesGraph);
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
            attributesGraph.Assert(new Triple(u_a, DCT_IDENTIFIER, n_a));
            attributesGraph.Assert(new Triple(u_a, RDF_VALUE, attributesGraph.CreateBlankNode(a.Name)));
        }

        private IUriNode createAttributeUriNode(AttributePrototype a, Graph g)
        {
            string attributeUri = dp_uri + "/" + a.Name;
            return g.CreateUriNode(attributeUri);
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

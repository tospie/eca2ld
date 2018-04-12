using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDPDatapoints;
using VDS.RDF;

namespace ECA2LD.ldp_ttl
{
    class AttributeLDPGraph : BasicLDPGraph
    {
        ECABaseModel.Attribute attribute;
        public AttributeLDPGraph(Uri u, ECABaseModel.Attribute a) : base(u)
        {
            attribute = a;
            BuildRDFGraph();
        }

        protected override void BuildRDFGraph()
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, LDP_RDF_RESOURCE));
            RDFGraph.Assert(new Triple(un, DCT_IDENTIFIER, RDFGraph.CreateLiteralNode(attribute.Definition.Name, "xsd:string")));

            string compUri = dp_uri.Replace("/" + attribute.Definition.Name, "");
            RDFGraph.Assert(new Triple(un, DCT_IS_PART_OF, RDFGraph.CreateUriNode(new Uri(compUri))));
            string valuestring = attribute?.Value?.ToString() ?? "null";
            RDFGraph.Assert(new Triple(un, RDF_VALUE, RDFGraph.CreateLiteralNode(valuestring, attribute.Type.transformTypeToString())));
        }
    }
}

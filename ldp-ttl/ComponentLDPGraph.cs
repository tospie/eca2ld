using ECABaseModel;
using ECABaseModel.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace ECA2LD.ldp_ttl
{
    public class ComponentLDPGraph : BasicLDPGraph
    {
        private Component c;
        private ILiteralNode n_c;
        private Uri u;

        public ComponentLDPGraph(Uri u, Component c) : base(u)
        {
            this.c = c;
            this.u = u;
            n_c = RDFGraph.CreateLiteralNode(c.Name, "xsd:string");
            BuildRDFGraph();
        }

        protected override void BuildRDFGraph()
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, LDP_BASIC_CONTAINER));
            RDFGraph.Assert(new Triple(un, DCT_IDENTIFIER, n_c));
            RDFGraph.Assert(new Triple(un, DCT_IS_PART_OF, GetContainingEntityURI()));
            RDFGraph.Assert(new Triple(un, LDP_HASMEMBERRELATION, DCT_HAS_PART));

            var definedByUri = new Uri(u.getPrototypeBaseUri() + c.Name + "/");
            RDFGraph.Assert(new Triple(un, RDFS_IS_DEFINED_BY, RDFGraph.CreateUriNode(definedByUri)));

            CreateAttributeTriples();
        }

        private IUriNode GetContainingEntityURI()
        {
            string entityUri = dp_uri.Replace("/" + c.Name, "");
            return RDFGraph.CreateUriNode(new Uri(entityUri));
        }

        private void CreateAttributeTriples()
        {
            foreach (AttributePrototype a in c.Definition.AttributeDefinitions)
            {
                string attributeUri = dp_uri.TrimEnd('/') + "/" + a.Name;
                RDFGraph.Assert(new Triple(un, DCT_HAS_PART, RDFGraph.CreateUriNode(new Uri(attributeUri))));
            }
        }
    }
}

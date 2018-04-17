using ECABaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace ECA2LD.ldp_ttl
{
    public class EntityLDPGraph : BasicLDPGraph
    {
        private ILiteralNode n_e;
        private Entity e;

        public EntityLDPGraph(Uri u, Entity e) : base(u)
        {
            this.e = e;
            n_e = RDFGraph.CreateLiteralNode(e.Guid.ToString(), "xsd:string");
            BuildRDFGraph();
        }

        protected override void BuildRDFGraph()
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, LDP_BASIC_CONTAINER));
            RDFGraph.Assert(new Triple(un, DCT_IDENTIFIER, n_e));
            RDFGraph.Assert(new Triple(un, LDP_HASMEMBERRELATION, DCT_HAS_PART));
            addComponentNodes();
        }

        private void addComponentNodes()
        {
            foreach (Component c in e.Components)
            {
                var componentUri = new Uri(dp_uri.TrimEnd('/') + "/" + c.Name);
                RDFGraph.Assert(new Triple(un, DCT_HAS_PART, RDFGraph.CreateUriNode(componentUri)));
            }
        }
    }
}

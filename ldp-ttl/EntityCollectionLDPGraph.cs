using ECA2LD.Datapoints;
using ECABaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECA2LD.ldp_ttl
{
    public class EntityCollectionLDPGraph : BasicLDPGraph
    {
        private EntityCollection Collection;

        public EntityCollectionLDPGraph(Uri u, EntityCollection collection) : base(u)
        {
            Collection = collection;
            collection.AddedEntity += (o, e) => Extend(e.Entity);
            BuildRDFGraph();
        }

        public void Extend(Entity e)
        {
            var dp = e.GetDatapoint();
            RDFGraph.Assert(new VDS.RDF.Triple(un, RDF_TYPE, LDP_BASIC_CONTAINER));
            RDFGraph.Assert(new VDS.RDF.Triple(un, RDFGraph.CreateUriNode("ldp:contains"), RDFGraph.CreateUriNode(new Uri(e.GetDatapoint().Route))));
        }

        protected override void BuildRDFGraph()
        {
            lock (Collection)
            {
                foreach (Entity e in Collection)
                {
                    Extend(e);
                }
            }
        }
    }
}

using ECA2LD.Datapoints;
using ECABaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

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
            RDFGraph.Assert(new Triple(un, RDF_TYPE, LDP_BASIC_CONTAINER));
            RDFGraph.Assert(new Triple(un, RDFGraph.CreateUriNode("ldp:contains"), RDFGraph.CreateUriNode(new Uri(e.GetDatapoint().Route))));
        }

        public void Extend(Graph g)
        {
            RDFGraph.Merge(g);
        }

        public void AddExternalContainer(string externalUri)
        {
            RDFGraph.Assert(new Triple(RDFGraph.CreateUriNode(new Uri(externalUri)), RDF_TYPE, LDP_BASIC_CONTAINER));
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

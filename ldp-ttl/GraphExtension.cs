using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace ECA2LD.ldp_ttl
{
    public static class GraphExtension
    {
        public static Graph CopyGraph(this Graph original)
        {
            Graph graphCopy = new Graph();
            foreach (Triple t in original.Triples)
            {
                Triple copy = t.CopyTriple(graphCopy);
                graphCopy.Assert(copy);
            }
            return graphCopy;
        }
    }
}

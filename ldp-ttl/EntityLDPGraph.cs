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

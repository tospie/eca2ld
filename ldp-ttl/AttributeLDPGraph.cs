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

        public AttributeLDPGraph(Uri u, ECABaseModel.Attribute a, string EntityUri) : base(u)
        {
            attribute = a;
            buildRDFBaseGraph();
            RDFGraph.Assert(new Triple(un, RDF_VALUE, RDFGraph.CreateUriNode(new Uri(EntityUri))));
        }

        public void AddTypeRoute(string typeRoute)
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, RDFGraph.CreateUriNode(new Uri(typeRoute))));
        }

        protected override void BuildRDFGraph()
        {
            buildRDFBaseGraph();
            RDFGraph.Assert(new Triple(un, RDF_VALUE, RDFGraph.CreateUriNode(new Uri(dp_uri + "/value/"))));
        }

        private void buildRDFBaseGraph()
        {
            RDFGraph.Assert(new Triple(un, RDF_TYPE, LDP_RDF_RESOURCE));
            RDFGraph.Assert(new Triple(un, DCT_IDENTIFIER, RDFGraph.CreateLiteralNode(attribute.Prototype.Name, new Uri("xsd:string"))));

            string compUri = dp_uri.Replace("/" + attribute.Prototype.Name, "");
            RDFGraph.Assert(new Triple(un, DCT_IS_PART_OF, RDFGraph.CreateUriNode(new Uri(compUri))));
        }
    }
}

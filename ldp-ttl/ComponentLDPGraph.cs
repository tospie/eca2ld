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
            foreach (AttributePrototype a in c.Prototype.AttributePrototypes)
            {
                string attributeUri = dp_uri.TrimEnd('/') + "/" + a.Name;
                RDFGraph.Assert(new Triple(un, DCT_HAS_PART, RDFGraph.CreateUriNode(new Uri(attributeUri))));
            }
        }
    }
}

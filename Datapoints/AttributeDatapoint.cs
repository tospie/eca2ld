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

using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDPDatapoints;
using ECA2LD.ldp_ttl;
using System.Reflection;
using LDPDatapoints.Subscriptions;

namespace ECA2LD.Datapoints
{
    /// <summary>
    /// Creates an ECA2LD HTTP / RDF endpoint around an <seealso cref="ECABaseModel.Attribute"/>
    /// </summary>
    class AttributeDatapoint : Resource
    {
        AttributeLDPGraph graph;
        object valueResource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attribute"><seealso cref="ECABaseModel.Attribute"/> that is to be represented by the RDF endpoint</param>
        /// <param name="uri">Endpoint listener URI</param>
        public AttributeDatapoint(ECABaseModel.Attribute attribute, string uri) : base(uri)
        {
            graph = new AttributeLDPGraph(new Uri(uri), attribute);
            Type valueResourceType = typeof(ValueResource<>).MakeGenericType(attribute.Type);
            WebsocketSubscription ws = new WebsocketSubscription(uri.Replace("http", "ws") + "/ws/");
            ConstructorInfo constructor = valueResourceType.GetConstructor(new Type[] { attribute.Type, typeof(string) });
            valueResource = constructor.Invoke(new object[] { attribute.Value, (uri + "/value/") });
            MethodInfo subscribe = valueResourceType.GetMethod("Subscribe");
            subscribe.Invoke(valueResource, new object[] { ws });
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {
            string graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }

        protected override void onPost(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void onPut(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

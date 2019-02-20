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
using VDS.RDF;
using LDPDatapoints.Events;

namespace ECA2LD.Datapoints
{
    internal static class AttributeDatapointManager
    {
        private static Dictionary<string, AttributeDatapoint> datapoints = new Dictionary<string, AttributeDatapoint>();

        public static void SetDatapoint(this ECABaseModel.Attribute attribute, AttributeDatapoint datapoint)
        {
            datapoints.Add(attribute.ParentComponent.Guid.ToString() + "." + attribute.Prototype.Name, datapoint);
        }

        public static AttributeDatapoint GetDatapoint(this ECABaseModel.Attribute attribute)
        {
            return datapoints[attribute.ParentComponent.Guid.ToString() + "." + attribute.Prototype.Name];
        }

        public static bool HasDatapoint(this ECABaseModel.Attribute attribute)
        {
            return datapoints.ContainsKey(attribute.ParentComponent.Guid.ToString() + "." + attribute.Prototype.Name);
        }
    }

    /// <summary>
    /// Creates an ECA2LD HTTP / RDF endpoint around an <seealso cref="ECABaseModel.Attribute"/>
    /// </summary>
    class AttributeDatapoint : Resource
    {
        internal AttributeLDPGraph graph;
        object valueResource;
        ECABaseModel.Attribute attribute;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attribute"><seealso cref="ECABaseModel.Attribute"/> that is to be represented by the RDF endpoint</param>
        /// <param name="uri">Endpoint listener URI</param>
        public AttributeDatapoint(ECABaseModel.Attribute attribute, string uri) : base(uri)
        {
            this.attribute = attribute;
            // In case we store an Entity as attribute, the resulting Resource should rather point to the datapoint that is created for it.
            bool isEntity = attribute.Type.Equals(typeof(ECABaseModel.Entity));
            bool isEntityCollection = attribute.Type.Equals(typeof(ECABaseModel.EntityCollection));

            // In case that the attribute contains another entity, we have to check whether there is already a Datapoint set up for it. If not,
            // we take care of this here. This follows the idea of automatic recursive Datapoint generation.
            if (isEntity && !((ECABaseModel.Entity)attribute.Value).HasDatapoint())
            {
                ECABaseModel.Entity child = (ECABaseModel.Entity)attribute.Value;
                var childEntityDP = new EntityDatapoint(child, this.Route.TrimEnd('/') + "/" + child.Guid + "/");
                var childGraph = childEntityDP.graph.RDFGraph;
                childGraph.Assert(new VDS.RDF.Triple(
                    childGraph.CreateUriNode(new Uri(this.Route.TrimEnd('/') + "/" + child.Guid + "/")),
                    childGraph.CreateUriNode("dct:isPartOf"),
                    childGraph.CreateUriNode(new Uri(attribute.ParentComponent.ContainingEntity.GetDatapoint().Route))
                    ));
            }

            if (isEntityCollection && !((ECABaseModel.EntityCollection)attribute.Value).HasDatapoint())
            {
                var child = (ECABaseModel.EntityCollection)attribute.Value;
                var childExDP = new EntityCollectionDatapoint(child, this.Route.TrimEnd('/') + "/" + child.Guid + "/");
            }

            // The RDF Graph vor the Attribute Node needs to point to this entity resource accordingly, instead of assuming a separate
            // attribute datapoint
            if (!(isEntity || isEntityCollection))
                graph = new AttributeLDPGraph(new Uri(uri), attribute);
            else if (isEntity)
                graph = new AttributeLDPGraph(new Uri(uri), attribute, ((ECABaseModel.Entity)attribute.Value).GetDatapoint().Route);
            else
                graph = new AttributeLDPGraph(new Uri(uri), attribute, ((ECABaseModel.EntityCollection)attribute.Value).GetDatapoint().Route);


            // if we have any other type of attribute, we go on to generate a datapoint for the attribute value based on the type of the attribute by
            // reflection
            if (!isEntity && !isEntityCollection)
            {
                Type valueResourceType = typeof(ValueResource<>).MakeGenericType(attribute.Type);
                Uri datapointUri = new Uri(uri);
                Uri wsUri = new Uri("ws://" + datapointUri.Host + ":" + (datapointUri.Port + 1) + datapointUri.PathAndQuery + "/ws/");
                WebsocketSubscription ws = new WebsocketSubscription(wsUri.ToString());
                ConstructorInfo constructor = valueResourceType.GetConstructor(new Type[] { attribute.Type, typeof(string) });
                valueResource = constructor.Invoke(new object[] { attribute.Value, (uri + "/value/") });
                EventInfo eventInfo = valueResourceType.GetEvent("ValueChanged");
                eventInfo.AddEventHandler(valueResource, new EventHandler<EventArgs>(HandleValueChanged));
                MethodInfo subscribe = valueResourceType.GetMethod("Subscribe");
                subscribe.Invoke(valueResource, new object[] { ws });
            }

            attribute.SetDatapoint(this);
        }

        private void HandleValueChanged(object sender, EventArgs e)
        {
            var valueArgs = e as ValueChangedEventArgs;
            if (valueArgs.Value != attribute.Value)
                attribute.Set(valueArgs.Value);
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {
            string graphAsTTL = graph.GetTTL();
            e.response.OutputStream.Write(Encoding.UTF8.GetBytes(graphAsTTL), 0, graphAsTTL.Length);
            e.response.OutputStream.Flush();
            e.response.OutputStream.Close();
        }

        protected override void onOptions(object sender, HttpEventArgs e)
        {
            throw new NotImplementedException();
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

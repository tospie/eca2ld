using ECABaseModel;
using LDPDatapoints;
using LDPDatapoints.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECA2LD.Datapoints
{
    class EntityCollectionDatapoint : ValueResource<EntityCollection>
    {
        public EntityCollectionDatapoint(EntityCollection value, string route) : base(value, route)
        {
            throw new NotImplementedException("Enity Collection Datapoints are not yet implemented");
        }

        protected override void onGet(object sender, HttpEventArgs e)
        {            
            base.onGet(sender, e);
        }
    }
}

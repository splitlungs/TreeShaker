using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace treeshaker
{
    public class TreeShakerMod : ModSystem
    {
        public ICoreAPI Api;
        
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            Api = api;

            Api.RegisterItemClass("TreeShakerItem", typeof(TreeShakerItem));
            Api.RegisterItemClass("ShakerMotorItem", typeof(ShakerMotorItem));
            Api.RegisterItemClass("ShakerHeadItem", typeof(ShakerHeadItem));
        }
    }
}

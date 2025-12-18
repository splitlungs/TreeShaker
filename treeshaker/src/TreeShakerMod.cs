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
        public ICoreServerAPI sApi;
        public ICoreClientAPI cApi;
        
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            Api = api;
            
            api.RegisterCollectibleBehaviorClass("TreeHarvestCollectibleBehavior", typeof(TreeHarvestCollectibleBehavior));
            api.RegisterItemClass("TreeShakerItem", typeof(TreeShakerItem));
            api.RegisterItemClass("ShakerMotorItem", typeof(ShakerMotorItem));
            api.RegisterItemClass("ShakerHeadItem", typeof(ShakerHeadItem));
        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            cApi = api;
            api.Logger.Event("[TreeShaker] Tree Shaker mod has been loaded.");
        }
        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            sApi = api;
            api.Logger.Event("[TreeShaker] Tree Shaker mod has been loaded.");
        }
    }
}

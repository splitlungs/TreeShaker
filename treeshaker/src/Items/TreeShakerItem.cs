using System;
using Vintagestory.API.Util;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace treeshaker
{
    public class TreeShakerItem : Item
    {
        ICoreAPI Api;
        public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity byEntity)
        {
            return "treeshaker-harvest1";
        }
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            Api = api;
        }
    }
}

using System;
using Vintagestory.API.Util;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;

namespace treeshaker
{
    public class ShakerHeadItem : Item
    {
        ICoreAPI Api;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            Api = api;
        }
    }
}

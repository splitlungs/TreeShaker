using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace treeshaker
{
    public static class FruitTreePartExtensions
    {
        private static readonly FieldInfo HarvestedField = typeof(BlockEntityFruitTreePart)
            .GetField("harvested", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void SetHarvested(this BlockEntityFruitTreePart part, bool? state)
        {
            part.FoliageState = EnumFoliageState.Plain;
            part.MarkDirty(redrawOnClient: true);
            HarvestedField?.SetValue(part, state);
        }
    }
}
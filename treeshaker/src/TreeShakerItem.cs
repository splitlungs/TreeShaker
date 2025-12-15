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
    public class TreeShakerItem : Item
    {
        ICoreAPI Api;
        private CollectibleBehaviorAnimationAuthoritative bhaa;
        private float harvestTime = 4f;
        private float lastHarvestSFX = 0f;
        private bool didHarvest = false;
        public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity byEntity)
        {
            return "treeshaker-harvest1";
        }
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            Api = api;
        }
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            if (!firstEvent) return;
            if (blockSel == null || byEntity == null) return;
            
            byEntity.AnimManager.StartAnimation("treeshaker-harvest1-fp");
            didHarvest = false;
            handling = EnumHandHandling.PreventDefault;
            return;
        }
        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (blockSel == null || byEntity == null) return false;

            // Limit SFX to once every 250 ms.
            if (Api.Side == EnumAppSide.Client && (secondsUsed - lastHarvestSFX) >= 0.25f)
            {
                Api.World.PlaySoundAt(new AssetLocation("game:sounds/block/plant"), byEntity.Pos.X, byEntity.Pos.Y, byEntity.Pos.Z);
                lastHarvestSFX = secondsUsed;
            }

            // Continue Steps
            if (secondsUsed < harvestTime) return true;

            // Completed Harvest
            didHarvest = true;
            return false;
        }
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            byEntity.AnimManager.StopAnimation("treeshaker-harvest1-fp");

            if (!didHarvest) return;
            if (blockSel == null || byEntity == null) 
            {
                didHarvest = false;
                return;
            }
            // Harvest and spawn drops
            HarvestAllFruitTreeParts(blockSel.Position, Api.World, byEntity as IPlayer);
            // Damage the tool by 1
            if (slot?.Itemstack != null)
            {
                slot.Itemstack.Collectible.DamageItem(Api.World, byEntity, slot, 1);
            }
            didHarvest = false;
        }
        /// <summary>
        /// Drops all fruit in a given radius of a target Fruit Tree stem.
        /// </summary>
        /// <param name="stemPos"></param>
        /// <param name="world"></param>
        /// <param name="player"></param>
        private void HarvestAllFruitTreeParts(BlockPos stemPos, IWorldAccessor world, IPlayer player)
        {
            int radius = 2;
            int height = 4;

            for (int dx = -radius; dx <= radius; dx++)
            for (int dy = 0; dy <= height; dy++)
            for (int dz = -radius; dz <= radius; dz++)
            {
                BlockPos pos = stemPos.AddCopy(dx, dy, dz);
                if (world.BlockAccessor.GetBlockEntity(pos) is not BlockEntityFruitTreePart part) continue;

                if (part.FoliageState != EnumFoliageState.Ripe) continue;

                // Mark as harvested
                part.FoliageState = EnumFoliageState.Plain;
                part.MarkDirty(true);

                // Get branch block from attribute
                string branchAttr = part.Block?.Attributes?["branchBlock"]?.AsString();
                if (string.IsNullOrEmpty(branchAttr)) continue;

                AssetLocation branchCode = AssetLocation.Create(branchAttr, part.Block.Code.Domain);
                if (world.GetBlock(branchCode) is not BlockFruitTreeBranch branchBlock) continue;

                if (!branchBlock.TypeProps.TryGetValue(part.TreeType, out var fruitProps)) continue;
                if (fruitProps?.FruitStacks == null) continue;

                foreach (BlockDropItemStack fruitStack in fruitProps.FruitStacks)
                {
                    ItemStack stack = fruitStack?.GetNextItemStack();
                    if (stack == null) continue;

                    if (player?.InventoryManager?.TryGiveItemstack(stack, true) != true)
                    {
                        world.SpawnItemEntity(stack, pos.ToVec3d().Add(0.5, 0.5, 0.5));
                    }

                    if (fruitStack.LastDrop) break;
                }
            }
        }
    }
}

using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace treeshaker
{
    public class TreeHarvestCollectibleBehavior : CollectibleBehavior
    {
        ICoreAPI Api;
        private int radius = 3;
        private int height = 5;
        private float harvestTime = 4.0f;
        private float harvestSFXFrequency = 0.25f;
        private float lastHarvestSFX = 0;
        public TreeHarvestCollectibleBehavior(CollectibleObject collObj) : base(collObj)
        {
            
        }
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            Api = api;
        }
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            // Safety checks
            if (!firstEvent) return;
            if (blockSel == null || byEntity == null) return;
            
            // Prepare for Steps
            lastHarvestSFX = 0f;
            byEntity.AnimManager.StartAnimation("treeshaker-harvest1-fp");

            handHandling = EnumHandHandling.PreventDefault;
            handling = EnumHandling.PreventDefault;

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling, ref handling);
        }
        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            if (blockSel == null || byEntity == null) return false;

            handling = EnumHandling.PreventDefault;
            
            // Limit SFX to once every 250 ms.
            float timeDiff = secondsUsed - lastHarvestSFX;
            if (timeDiff >= harvestSFXFrequency)
            {
                byEntity.Api.World.PlaySoundAt(new AssetLocation("game:sounds/block/plant"), byEntity.Pos.X, byEntity.Pos.Y, byEntity.Pos.Z);
                lastHarvestSFX = secondsUsed;
            }
            // Continue Steps
            if (secondsUsed < harvestTime) return true;
            // Continue to Stop
            return false;
        }
        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason, ref EnumHandling handled)
        {
            handled = EnumHandling.PreventDefault;
            byEntity.AnimManager.StopAnimation("treeshaker-harvest1-fp");
            lastHarvestSFX = 0f;
            return true;
        }
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            // Safety checks & Cleanup
            if (secondsUsed < harvestTime) return;
            handling = EnumHandling.PreventDefault;
            if (byEntity == null) return;
            byEntity.AnimManager.StopAnimation("treeshaker-harvest1-fp");
            lastHarvestSFX = 0f;
            if (blockSel == null || slot?.Itemstack == null) return;

            if (!(Api is ICoreServerAPI sapi)) return;
            // Claims Check
            EntityPlayer ep = byEntity as EntityPlayer;
            IPlayer player = sapi.World.PlayerByUid(ep.PlayerUID);
            // Check Land Claims.
            LandClaim[] claims = sapi.World.Claims.Get(blockSel.Position);
            if (claims != null)
            {
                foreach (LandClaim lc in claims)
                {
                    var response = lc.TestPlayerAccess(player, EnumBlockAccessFlags.BuildOrBreak);
                    if (response < EnumPlayerAccessResult.OkOwner) 
                    {
                        handling = EnumHandling.PreventDefault;
                        sapi.Logger.Audit("[TreeShaker] {0} has been denied access due to a land claim at {1}.", player.PlayerName, blockSel.Position);
                        return;
                    }
                }
            }
            // Harvest if no claim violations
            HarvestAllFruitTreeParts(blockSel.Position, byEntity.Api.World, byEntity as IPlayer);
            sapi.Logger.Audit("[TreeShaker] {0} has harvested a tree at {1}.", player.PlayerName, blockSel.Position);
            // Damage the tool by 1
            slot.Itemstack.Collectible.DamageItem(byEntity.Api.World, byEntity, slot, 1);
        }
        /// <summary>
        /// Drops all fruit in a given radius of a target Fruit Tree stem.
        /// </summary>
        /// <param name="stemPos"></param>
        /// <param name="world"></param>
        /// <param name="player"></param>
        private void HarvestAllFruitTreeParts(BlockPos stemPos, IWorldAccessor world, IPlayer player)
        {
            // IBulkBlockAccessor bba = Api.World.GetBlockAccessorBulkUpdate(true, true);
            // bba.WalkBlocks(stemPos, stemPos, onBlock, false);
            // bba.Commit();

            // NEVER BELIEVE THE MACHINES
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

                var gd = part.GrowthDir;

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
                part.MarkDirty(true);
            }
        }
        public void onBlock(Block block, int i, int j, int k)
        {
            
        }
    }
}
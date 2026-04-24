using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace treeshaker
{
    /// <summary>
    /// This class is a graveyard of what once was. There is a new class in town.
    /// </summary>
    [Obsolete]
    public class TreeShakerItemOLD : Item
    {
        private Room RoomForInteract;
        private RoomRegistry RoomReg;
        private EntityPartitioning ep;
        private List<BlockEntityFruitTreePart> TreeFoliage = new List<BlockEntityFruitTreePart>();
        private List<BlockEntityFruitTreePart> TreeBranches = new List<BlockEntityFruitTreePart>();
        private List<BlockEntityFruitTreePart> TreeStems = new List<BlockEntityFruitTreePart>();
        //private List<BlockEntity> TreeParts = new List<BlockEntity>();
        //private List<Entity> TreeEnts = new List<Entity>();
        private float SecondsUsed = 0f;
        private bool CanCollect = false;
        private bool IsCollecting = false;
        // Knife harvesting speed is equivalent to 50% of the plant breaking speed bonus
        public float HarvestingSpeed = 3f;
        public int HeightMax = 10;
        public int BranchMax = 3;
        public string treeshakerHitBlockAnimation;
        public string treeshakerHitEntityAnimation;
        ICoreAPI API;
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            RoomReg = api.ModLoader.GetModSystem<RoomRegistry>();
            treeshakerHitBlockAnimation = Attributes["knifeHitBlockAnimation"].AsString(HeldTpHitAnimation);
            treeshakerHitEntityAnimation = Attributes["knifeHitEntityAnimation"].AsString(HeldTpHitAnimation);

            ep = api.ModLoader.GetModSystem<EntityPartitioning>();
            API = api;
        }
        public override string GetHeldTpHitAnimation(ItemSlot slot, Entity byEntity)
        {
            if ((byEntity as EntityPlayer)?.EntitySelection != null)
            {
                return treeshakerHitEntityAnimation;
            }
            
            if ((byEntity as EntityPlayer)?.BlockSelection != null)
            {
                return treeshakerHitBlockAnimation;
            }

            return base.GetHeldTpHitAnimation(slot, byEntity);
        }
        #region OLD Interact
        /*
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            api.World.GetBlockAccessorPrefetch(true, true);
            // StartInteract(blockSel, entitySel, firstEvent);

            handling = EnumHandHandling.PreventDefault;

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }
        protected void StartInteract(BlockSelection blockSel, EntitySelection entitySel, bool firstEvent)
        {

            // Sanity Check
            // if (firstEvent == false || blockSel == null || api.World.Side == EnumAppSide.Server)
                // return;

            EnumHandHandling bhHandHandling = EnumHandHandling.PreventDefault;
            API.Logger.Debug("Interacting!");

            // Confirm player interacted with a real block
            if (!(entitySel?.Entity != null))
                return;
            API.Logger.Debug("Entity found!");

            // Confirm that block was a Tree Part (Stem)
            var beftp = API.World.BlockAccessor.GetBlockEntity(entitySel.Position.AsBlockPos) as BlockEntityFruitTreePart;
            if (beftp == null)
                return;
            API.Logger.Debug("Fruit Tree Part found!");

            // Sanity check the tree's type
            if (beftp.TreeType == null)
            {
                API.World.Logger.Error("Coding error. Fruit tree without fruit tree type @" + entitySel.Position);
                return;
            }

            // Start from the stem thep layer is interacting with
            TreeStems.Clear();
            TreeStems.Add(beftp);

            //while (api.World.BlockAccessor.GetBlock(beftp.Pos.X, beftp.Pos.Y + faceY, beftp.Pos.Z).Id != 0)
            // api.World.BlockAccessor.WalkBlocks(1, 1, );
            
            // BlockPos ePos = byEntity.Pos.AsBlockPos;
            // // Check a 7x7x7 area for logs
            // int quantityLogs = 0;
            // api.World.BlockAccessor.WalkBlocks(
            //     ePos.AddCopy(-3, -3, -3),
            //     ePos.AddCopy(3, 3, 3),
            //     (block, x, y, z) => quantityLogs += block.Code.Path.Contains("log") ? 1 : 0
            // );


            // Climb UP the stem
            // for (int i = 1; i < HeightMax; i++)
            // {
            //     api.Logger.Debug("Looping stems!");
            //     // api.World.BlockAccessor.WalkBlocks(beftp.Pos, beftp.Pos, (block, x, y, z) => block.Code.Path.Contains("fruittree") ? 1 : 0 , true);
            //     var stemEnt = api.World.BlockAccessor.GetBlockEntity(new BlockPos(beftp.Pos.X, beftp.Pos.Y + i, beftp.Pos.Z, 1)) as BlockEntityFruitTreePart;
            //     if (stemEnt != null)
            //         if (stemEnt.PartType == EnumTreePartType.Stem)
            //             TreeStems.Add(stemEnt);
            // }
            // api.Logger.Debug("Total stems: " + TreeStems.Count());
            // // Horizontal Branches
            // TreeBranches.Clear();
            // for (int i = 0; i < TreeStems.Count; i++)
            // {
            //     for (int j = 0; j < 4; j++)
            //     {
            //         var face = BlockFacing.HORIZONTALS[j];
            //         if (api.World.BlockAccessor.GetBlock(new BlockPos(TreeStems[i].Pos.X + face.Normali.X, TreeStems[i].Pos.Y, TreeStems[i].Pos.Z + face.Normali.Z, 1)).Id != 0)
            //         {
            //             var branchEnt = api.World.BlockAccessor.GetBlockEntity(new BlockPos(TreeStems[i].Pos.X + face.Normali.X, TreeStems[i].Pos.Y, TreeStems[i].Pos.Z + face.Normali.Z, 1)) as BlockEntityFruitTreePart;
            //             if (branchEnt != null)
            //                 if (branchEnt.PartType == EnumTreePartType.Branch)
            //                     TreeBranches.Add(branchEnt);
            //         }
            //     }
            // }
            // api.Logger.Debug("Total branches: " + TreeBranches.Count());
            // // Returning null for some reason?
            // FruitTreeRootBH rootBH = beftp.GetBehavior<FruitTreeRootBH>();
            // // FruitTreeTypeProperties typeProps;
            // // api.Logger.Debug("Blocks grown: " + rootBH.BlocksGrown);
            // // ItemStack parentStack = rootBH.propsByType.TryGetValue("");
            // 
            // // Start all the stems
            // foreach (var treeStem in TreeStems)
            // {
            //     //treeStem.OnBlockInteractStart(byEntity.api as IPlayer, blockSel);
            // }
            // // Start all the branches
            // foreach (var treeBranches in TreeBranches)
            // {
            //     //treeBranches.OnBlockInteractStart(byEntity.api as IPlayer, blockSel);
            // }

            // TEMP FOR DIAGNOSTICS
            //ep.WalkEntities(entitySel.Position, 5d, )
            // List<BlockEntityFruitTreePart> TreeParts = new List<BlockEntityFruitTreePart>();
            // BlockEntityFruitTreePart TreeRoot = API.World.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntityFruitTreePart;
            // TreeParts.Add(TreeRoot);
            // FruitTreeRootBH rootBH = TreeRoot.GetBehavior<FruitTreeRootBH>();
            // RoomReg = API.ModLoader.GetModSystem<RoomRegistry>();
            // RoomForInteract = RoomReg.GetRoomForPosition(TreeRoot.Pos);
            // Cuboidi cube = RoomForInteract.Location;
            // Entity[] TreeEnts = API.World.GetEntitiesInsideCuboid(cube.Start.ToBlockPos(), cube.End.ToBlockPos());
            // API.Logger.Debug("Start " + cube.Start + "; End " + cube.End);
            // foreach (Entity ent in TreeEnts)
            // {
            //     BlockEntityFruitTreePart tempBE = API.World.BlockAccessor.GetBlockEntity(ent.Pos.AsBlockPos) as BlockEntityFruitTreePart;
            //     if (tempBE != null)
            //         TreeParts.Add(tempBE);
            // }
            // API.Logger.Debug("There are {0} Tree Parts", TreeParts.Count);
            
            // api.Logger.Debug("Blocks grown {0}", rootBH.BlocksGrown);
            // ICoreServerAPI sApi = api as ICoreServerAPI;
            // ICoreClientAPI cApi = api as ICoreClientAPI;
            // sApi.World.BlockAccessor.SpawnBlockEntity(TreeRoot);

            // // Get the Tree's Root Block Entity
            // var rootBe = (api.World.BlockAccessor.GetBlockEntity(beftp.Pos.AddCopy(beftp.RootOff)) as BlockEntityFruitTreeBranch);
            // if (rootBe == null)
            //     return;
            // api.Logger.Debug("Root Block Entity found!");
            // // Check for Ripeness
            // if (beftp.FruitTreeState != EnumFruitTreeState.Ripe)
            //     return;
            // api.Logger.Debug("BlockEntity is a Ripe Tree!");
            // // Get the Room to find the other parts?
            // RoomReg = api.ModLoader.GetModSystem<RoomRegistry>();
            // RoomForInteract = RoomReg.GetRoomForPosition(rootBe.Pos);
            // if (RoomForInteract == null)
            //     return;
            // api.Logger.Debug("Tree's Room found!");
            // Cuboidi cube = RoomForInteract.Location;
            // TreeEnts.Clear();
            // TreeEnts.AddRange(api.World.GetEntitiesInsideCuboid(new BlockPos(cube.Start), new BlockPos(cube.End)));
            // //beftp.RootOff
            // // api.World.BlockAccessor.GetBlockEntity<>
            // // TreeEnts.AddRange(api.World.GetEntitiesInsideCuboid(new BlockPos(cube.Start), new BlockPos(cube.End), (e) => !(e is BlockEntity)));
            // api.Logger.Debug("TreeEnts: " + TreeEnts.Count);
            // 
            // TreeParts.Clear();
            // foreach (var entity in TreeEnts)
            // {
            //     TreeParts.Add(api.World.BlockAccessor.GetBlockEntity(entity.Pos.AsBlockPos));
            // }
            // api.Logger.Debug("TreeParts: " + TreeParts.Count);
            // for (int i = 0; i < TreeParts.Count - 1; i++)
            // {
            //     var ftrbh = TreeParts[i].GetBehavior<FruitTreeRootBH>();
            //     if (ftrbh == null)
            //         return;
            //     api.Logger.Debug("TreePart's Behavior found!");
            // }
            // 

            // CanCollect = true;
            // handling = bhHandHandling;
        }
        public void LinkTreePart()
        {

        }
        public override bool OnHeldInteractStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
        {
            if (CanCollect == false)
                return base.OnHeldAttackStep(secondsPassed, slot, byEntity, blockSelection, entitySel);

            foreach (var treeStem in TreeStems)
            {
                //treeStem.OnBlockInteractStop(secondsPassed, byEntity.api as IPlayer, blockSelection);
            }

            foreach (var treeBranch in TreeBranches)
            {
                //treeBranch.OnBlockInteractStop(secondsPassed, byEntity.api as IPlayer, blockSelection);
            }

            // EntityBehaviorHarvestable bh;
            // if (entitySel != null && (bh = entitySel.Entity.GetBehavior<EntityBehaviorHarvestable>()) != null && bh.Harvestable)
            // {
            //     if (byEntity.World.Side == EnumAppSide.Client)
            //     {
            //         ModelTransform tf = new ModelTransform();
            //         tf.EnsureDefaultValues();
            //         tf.Translation.Set(0, 0, -Math.Min(0.6f, SecondsUsed * 2));
            //         tf.Rotation.Y = Math.Min(20, SecondsUsed * 90 * 2f);
            //         if (SecondsUsed > 0.4f)
            //         {
            //             tf.Translation.X += (float)Math.Cos(SecondsUsed * 15) / 10;
            //             tf.Translation.Z += (float)Math.Sin(SecondsUsed * 5) / 30;
            //         }
            //         byEntity.Controls.UsingHeldItemTransformBefore = tf;
            //     }
            //     return SecondsUsed < HarvestingSpeed * bh.GetHarvestDuration(byEntity) + 0.15f;
            // }
            return false;
        }
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            foreach (var treeStem in TreeStems)
            {
                //treeStem.OnBlockInteractStop(secondsUsed, byEntity.api as IPlayer, blockSel);
            }
            foreach (var treeBranch in TreeBranches)
            {
                //treeBranch.OnBlockInteractStop(secondsUsed, byEntity.api as IPlayer, blockSel);
            }
            // FruitTreeRootBH bh = blockSel.Block.GetBEBehavior<FruitTreeRootBH>(blockSel.Position);
            // // Confirm player interacted with a real block
            // if (blockSel.Block == null)
            //     return;
            // api.Logger.Debug("Block found!");
            // // Confirm that block was a Tree Part (Stem)
            // var beftp = blockSel.Block.GetBlockEntity<BlockEntityFruitTreePart>(blockSel.Position);
            // //BlockEntityFruitTreePart beftp = blockSel.Block.GetBlockEntity<BlockEntityFruitTreePart>(blockSel.Position);
            // if (beftp == null)
            //     return;
            // api.Logger.Debug("Fruit Tree Part found!");
            // if (secondsUsed > 1.1 && beftp.FoliageState == EnumFoliageState.Ripe)
            // {
            //     beftp.FoliageState = EnumFoliageState.Plain;
            //     beftp.MarkDirty(true);
            //     //beftp.harvested = true;
            //     var loc = AssetLocation.Create(Block.Attributes["branchBlock"].AsString(), Block.Code.Domain);
            //     var block = api.World.GetBlock(loc) as BlockFruitTreeBranch;
            //     var drops = block.TypeProps[TreeType].FruitStacks;
            //     foreach (var drop in drops)
            //     {
            //         ItemStack stack = drop.GetNextItemStack(1);
            //         if (stack == null) continue;
            //         if (!byPlayer.InventoryManager.TryGiveItemstack(stack, true))
            //         {
            //             api.World.SpawnItemEntity(stack, byPlayer.Entity.Pos.XYZ.Add(0, 0.5, 0));
            //         }
            //         if (drop.LastDrop) break;
            //     }
            // }
        }
        public override bool OnHeldInteractCancel(float SecondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            return false;
        }
        */
        #endregion

        #region RoboCode
    #endregion
    }
}

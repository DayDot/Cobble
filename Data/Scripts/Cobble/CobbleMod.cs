using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using Sandbox.Game;
using Sandbox.Game.Entities;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Input;
using VRageMath;
using VRage;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.Utils;
using Sandbox.Definitions;
using VRage.Voxels;
using VRage.Game.Voxels;
using Medieval.GameSystems;

namespace DayDot.Cobble
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class CobbleMod : MySessionComponentBase
    {
        public override void LoadData()
        {
            Log.SetUp("Daydot Cobble", 827470055, "DayDot.Cobble");
        }

        public const ushort PACKET = 8274;

        public static CobbleMod instance = null;
        public static bool init { get; private set; }
        public static bool isThisDedicated { get; private set; }
        private static readonly List<IMyVoxelBase> maps = new List<IMyVoxelBase>();

        public MyHandToolBase holdingTool = null;
        public long ShootTick = 0;
        public const long ShootTickConst = 35;
        public const long ShootTickMax   = 90;
        private IMyHudNotification toolStatus = null;

        private static MyVoxelMaterialDefinition cobble = null;
        private static MyVoxelMaterialDefinition dirt = null;

        public const string CobbleName = "Cobble";
        public const string DirtName   = "Soil";

        private const double VoxelChangeRadius = 1.0;
        private const double VoxelChangeRadiusSq = VoxelChangeRadius * VoxelChangeRadius;
        protected MyStorageData m_cache = new MyStorageData(MyStorageDataTypeFlags.ContentAndMaterial);

        enum MouseButton
        {
            None, Right, Left
        };

        private MouseButton pressedButton = MouseButton.None;

        protected bool[] m_paveableMaterials;

        public override void BeforeStart()
        {
            base.BeforeStart();
            this.m_paveableMaterials = new bool[256];
            for (int index = 0; index < 256; ++index)
                this.m_paveableMaterials[index] = false;

            var PaveableMaterialNames = new List<String>{
                "Grass",
                "Grass_02",
                "Grass_old",
                "Woods_needles",
                "Woods_grass",
                "Soil",
                "PlowedSoil",
                "PlantedSoil",
                "ExhaustedSoil"};

            foreach (string plowableMaterial in PaveableMaterialNames)
                this.m_paveableMaterials[(int)MyDefinitionManager.Static.GetDefinition<MyVoxelMaterialDefinition>(plowableMaterial).Index] = true;
        }

        public void DrawTool(MyHandToolBase gun)
        {
            holdingTool = gun;
        }

        public void Init()
        {
            instance = this;
            Log.Init();

            init = true;
            isThisDedicated = (MyAPIGateway.Utilities.IsDedicated && MyAPIGateway.Multiplayer.IsServer);

            //collect voxels
            if (cobble == null)
            {
                cobble = MyDefinitionManager.Static.GetVoxelMaterialDefinition(CobbleName);
                if (cobble == null)
                    Log.Info("No cobble material found");

            }

            if (dirt == null)
            {
                dirt = MyDefinitionManager.Static.GetVoxelMaterialDefinition(DirtName);
                if (dirt == null)
                    Log.Info("No soil material found");
            }

            MyAPIGateway.Session.VoxelMaps.GetInstances(maps, delegate (IMyVoxelBase map_)
            {
                if (map_.StorageName == null)
                    return false;

                return true; //there is only one, since it's medieval engineers.
            });

            if (maps.Count == 0)
            {
                var str = "No voxel maps defined";
                Log.Error(str, str);
                return;
            }

        }

        protected override void UnloadData()
        {
            try
            {
                if (init)
                {
                    init = false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            Log.Close();
        }

        public override void UpdateAfterSimulation()
        {
            try
            {
                if (!init)
                {
                    if (MyAPIGateway.Session == null)
                        return;

                    Init();
                }

                if (isThisDedicated)
                    return;

                if (holdingTool == null || holdingTool.Closed || holdingTool.MarkedForClose)
                    return;

                var character = MyAPIGateway.Session.ControlledObject as IMyCharacter;

                if (character == null)
                    return;

                var shoot = holdingTool.IsShooting;
                if (shoot)
                {
                    ShootTick++;
                    var input = MyAPIGateway.Input;
                    if (input.IsLeftMousePressed())
                        pressedButton = MouseButton.Left;
                    else if (input.IsRightMousePressed())
                        pressedButton = MouseButton.Right;

                }
                else
                {
                    pressedButton = MouseButton.None;
                    ShootTick = 0;
                }

                if (ShootTick == ShootTickConst)
                {
                    Shoot();
                    ShootTick++;
                }
                else if (ShootTick == ShootTickMax)
                    ShootTick = 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void Shoot()
        {

            var character = MyAPIGateway.Session.ControlledObject as IMyCharacter;
            if (character == null)
                return;

            if (maps.Count == 0)
                return;

            var pos = character.GetPosition();
            var map = maps[0];

            var view = character.GetHeadMatrix(true, true);
            var target = view.Translation + view.Forward;


            var done = ToolProcess(map, target, view);
        }

        private bool ToolProcess(IMyVoxelBase map, Vector3D target, MatrixD view)
        {
            
            var shape = MyAPIGateway.Session.VoxelMaps.GetBoxVoxelHand();
            var vec = (Vector3D.One / 2) * 2.0;
            var bounding = new BoundingBoxD(-vec, vec);
            shape.Boundaries = bounding; // new BoundingSphere(0, 1.0f);
            IMyVoxelShape placeShape = shape;

            var grid = MyAPIGateway.CubeBuilder.FindClosestGrid();

            MatrixD placeMatrix = MatrixD.Identity;

            if (grid != null)
                placeMatrix = grid.WorldMatrix;
            else
            {
                var planet = map as MyPlanet;
                var center = (planet != null ? planet.WorldMatrix.Translation : map.PositionLeftBottomCorner + (map.Storage.Size / 2));
                var dir = (target - center);
                placeMatrix = MatrixD.CreateFromDir(dir, view.Forward);
            }

            placeMatrix.Translation = target;
            
            placeShape.Transform = placeMatrix;
            

            byte materialIndex = dirt.Index;
            if ((pressedButton == MouseButton.Left) && (cobble != null))
                materialIndex = cobble.Index;

            MyAPIGateway.Session.VoxelMaps.PaintInShape(map, placeShape, materialIndex);
            return true;
        }
    }
}
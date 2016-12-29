using Medieval.Definitions.Tools;
using Medieval.GameSystems;
using Medieval.Entities;
using Medieval;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.Gui;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;
using VRage.Game.Components;

namespace DayDot.Cobble
{ 
  
//   public class MyPavingToolComponent : MyGameLogicComponent
//   {
//     private MyStringId m_editVoxelsId = MyStringId.GetOrCompute("VoxelEdit");
//     //private readonly MyPaveToolDefinition m_plowItemDefinition;

//     public MyPavingToolComponent(MyToolItemDefinition toolItemDefinition)
//     {
//     //  this.m_plowItemDefinition = toolItemDefinition as MyPaveToolDefinition;

//     }

//     public bool Hit(MyEntity entity, MyHitInfo hitInfo, uint shapeKey, float efficiency)
//     {
//     /*  if (this.m_plowItemDefinition == null)
//       {
//         MyLog.Default.WriteLine("Wrong definition set in constructor. Please check.");
//         return false;
//       }*/
//       if (!(entity is MyVoxelBase))
//       {
//         MyLog.Default.WriteLine("Incorrect target (which should be voxel). Should fail already on GetStateForTarget(). Please check.");
//         return false;
//       }
//       bool flag = false;
//    /*   if (this.m_toolOwner != null)
//       {
//         long playerIdentityId = this.m_toolOwner.GetPlayerIdentityId();
//         if (MyAreaPermissionSystem.Static != null)
//           flag = MyAreaPermissionSystem.Static.HasPermission(playerIdentityId, hitInfo.Position, this.m_editVoxelsId);
//       }
//       if (flag || this.m_toolOwner == null || this.m_toolOwner != MySession.Static.LocalCharacter)
//           return this.Cobble((MyEntity) this.m_toolOwner, hitInfo.Position);
// */
//       return false;
//     }

//     public bool Cobble(MyEntity planter, Vector3D position)
//     {
//       MyPlanet closestPlanet = MyGamePruningStructure.GetClosestPlanet(position);
//       if (closestPlanet == null )
//         return false;
//       //if (Sync.IsServer)
//       if (true)
//       {
//       /*  this.ChangeMaterialsForFarm((MyVoxelBase) closestPlanet, position, this.PlowedMaterial.Index, true);
//         MyFarmableEnvironmentModule proxyForPosition = this.GetFarmableProxyForPosition(closestPlanet, position);
//         if (proxyForPosition != null)
//           proxyForPosition.Plow(planter, position);
//         MyMultiplayer.RaiseStaticEvent<long, long, Vector3D, byte>((Func<IMyEventOwner, Action<long, long, Vector3D, byte>>) (x => new Action<long, long, Vector3D, byte>(MyFarmingSystem.OnPlow)), planter.EntityId, closestPlanet.EntityId, position, this.PlowedMaterial.Index, new EndpointId());
//         */
//       }
//       return true;
//     }


//     public void Shoot()
//     {
//     }

//     public void Update()
//     {
//     }

//     public void DrawHud() {}

//     public void OnControlReleased() 
//     {

//     }

//     /*public void OnControlAcquired(MyCharacter owner)
//     {
//     //  this.m_toolOwner = owner;
//     }*/



//     public string GetStateForTarget(MyEntity targetEntity, uint shapeKey, Vector3D hitPosition)
//     {
//       if (targetEntity is MyVoxelBase)
//         return "Voxel";
//       return (string) null;
//     }
//     public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
//     {
//         return Entity.GetObjectBuilder(copy);
//     }
//   }
}
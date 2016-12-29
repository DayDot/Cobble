using System;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game;

namespace DayDot.Cobble
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_HandTool), "PavingHammer")]
    public class Tool : MyGameLogicComponent
    {
        private bool first = true;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
        }

        public override void UpdateBeforeSimulation()
        {
            try
            {
                if (first)
                {
                    var mod = CobbleMod.instance;

                    if (mod == null)
                        return;

                    first = false; // don't move this up because it needs to repeat until mod and character are available for a valid check
                    var gun = Entity as Sandbox.Game.Entities.MyHandToolBase;
                        // IMyAutomaticRifleGun;
                    //Medieval.Entities.MyHandTool
                   
                    if (gun?.Owner != null)// && gun.Owner.Id == MyAPIGateway.Session.Player.PlayerID) // check if the local player is holding it
                    {
                        mod.DrawTool(gun);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return Entity.GetObjectBuilder(copy);
        }
       
}
}

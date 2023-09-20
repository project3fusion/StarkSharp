using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct ResetInputSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Create a new entity command buffer
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            
            //Get Targeter Tag and Projectile Entity from all open Targeter Tag entities
            foreach(var (targeterTag, projectileEntity) in SystemAPI.Query<TargeterTag>().WithEntityAccess())
            {
                //Set targeter tag to false
                ecb.SetComponentEnabled<TargeterTag>(projectileEntity, false);
            }

            //Play Entity Command Buffer
            ecb.Playback(state.EntityManager);

            //Dispose Entity, free memory
            ecb.Dispose();
        }
    }
}
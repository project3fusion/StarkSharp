using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerFireProjectileSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Create a new entity command buffer and allocate memory
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            //Foreach each Targeter Tag take Projectile Prefab Data and Local Transform
            foreach(var (projectilePrefabData, localTransform) in SystemAPI.Query<ProjectilePrefabData, LocalTransform>().WithAll<TargeterTag>())
            {
                //Instantiate new object with Entity Command Buffer
                var newProjectile = ecb.Instantiate(projectilePrefabData.Value);

                //Create Projectile Local Transform from the transform of the Targeter Tag Entity (Player)
                var projectileLocalTransform = LocalTransform.FromPositionRotationScale(
                    localTransform.Position, 
                    localTransform.Rotation, 
                    0.5f
                );

                //Set local transform of New Projectile to Projectile Local Transform
                ecb.SetComponent(newProjectile, projectileLocalTransform);
            }

            //Play Entity Command Buffer
            ecb.Playback(state.EntityManager);

            //Dispose Entity, free memory
            ecb.Dispose();
        }
    }
}

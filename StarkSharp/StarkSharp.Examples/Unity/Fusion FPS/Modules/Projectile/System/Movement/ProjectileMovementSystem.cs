using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct ProjectileMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Get delta time
            var deltaTime = SystemAPI.Time.DeltaTime;

            //Create a new Projectile Movement Job and Schedule
            new ProjectileMovementJob
            {
                deltaTime = deltaTime,
            }.Schedule();
        }
    }
}

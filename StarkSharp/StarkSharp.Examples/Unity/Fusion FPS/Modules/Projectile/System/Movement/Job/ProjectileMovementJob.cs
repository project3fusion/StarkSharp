using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    public partial struct ProjectileMovementJob : IJobEntity
    {
        public float deltaTime;

        //in ProjectileTag projectileTag might not be used for better performance.
        [BurstCompile]
        private void Execute(in ProjectileTag projectileTag, ref LocalTransform localTransform, in ProjectileMovementSpeedData projectileMovementSpeedData)
        {
            //Update local transform position by forward direction * speed * delta time
            localTransform.Position += localTransform.Forward() * projectileMovementSpeedData.Value * deltaTime;
        }
    }
}
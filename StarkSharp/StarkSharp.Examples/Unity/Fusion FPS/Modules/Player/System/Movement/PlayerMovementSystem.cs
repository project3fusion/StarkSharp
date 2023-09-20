using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Get delta time (Time between two frames)
            var deltaTime = SystemAPI.Time.DeltaTime;

            //Create a new Player Movement Job and Schedule
            new PlayerMovementJob
            {
                deltaTime = deltaTime,
            }.Schedule();
        }
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Get delta time (Time between two frames)
            var deltaTime = SystemAPI.Time.DeltaTime;

            //Create a new Player Rotation Job and Schedule
            new PlayerRotationJob
            {
                deltaTime = deltaTime,
            }.Schedule();
        }
    }
}

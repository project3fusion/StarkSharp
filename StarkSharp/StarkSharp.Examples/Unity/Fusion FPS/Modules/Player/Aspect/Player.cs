using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    public readonly partial struct Player : IAspect
    {
        private readonly RefRO<LocalTransform> playerLocalTransformData;
        private readonly RefRO<PlayerMovementInputData> playerMovementInputData;
        private readonly RefRO<PlayerMovementSpeedData> playerMovementSpeedData;

        public float playerMovementSpeed => playerMovementSpeedData.ValueRO.Value;
        public float2 playerMovementInput => playerMovementInputData.ValueRO.Value;
        public LocalTransform playerLocalTransform => playerLocalTransformData.ValueRO;
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    public partial struct PlayerRotationJob : IJobEntity
    {
        public float deltaTime;

        [BurstCompile]
        private void Execute(ref LocalTransform localTransform, in PlayerRotationInputData playerRotationInputData, in PlayerRotationSpeedData playerRotationSpeedData)
        {
            float angleX = playerRotationInputData.Value.x * deltaTime * playerRotationSpeedData.Value;

            localTransform = localTransform.RotateY(angleX);
        }
    }
}

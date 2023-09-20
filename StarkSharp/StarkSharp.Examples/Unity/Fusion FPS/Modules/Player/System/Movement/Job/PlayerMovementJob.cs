using ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float deltaTime;

        [BurstCompile]
        private void Execute(ref LocalTransform localTransform, in PlayerMovementInputData playerMovementInputData, in PlayerRotationSpeedData playerRotationSpeedData)
        {
            //Get input direction from player movement input data
            float3 inputDirection = math.normalize(new float3(playerMovementInputData.Value.x, 0, playerMovementInputData.Value.y));

            //If input direction length is not greater than threshold value set it to zero
            float3 finalDirection = math.length(inputDirection) > 0.25f ? inputDirection : float3.zero;

            //Get final direction with rotating direction to the player's rotation
            finalDirection = math.rotate(localTransform.Rotation, finalDirection);

            //Get amount of movement that will be applied to the local transform position
            float3 movementOffset = finalDirection * playerRotationSpeedData.Value * deltaTime;

            //Create a new local transform with new movement offset.
            localTransform = LocalTransform.FromPositionRotationScale(
                localTransform.Position + movementOffset,
                localTransform.Rotation,
                localTransform.Scale
            );
        }
    }
}
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    public partial struct CameraMovementJob : IJobEntity
    {
        public float deltaTime;
        public float3 playerPosition;
        public quaternion playerRotation;

        private void Execute(ref LocalTransform localTransform, in CameraTag cameraTag, in CameraPositionData cameraPositionData)
        {
            UnityEngine.Debug.Log("hello!!");
            localTransform.Position = playerPosition;
            localTransform.Rotation = playerRotation;
        }
    }
}

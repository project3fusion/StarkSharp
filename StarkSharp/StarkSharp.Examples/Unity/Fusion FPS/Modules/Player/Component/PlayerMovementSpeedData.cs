using Unity.Entities;
using Unity.Mathematics;

namespace ECS
{
    public struct PlayerMovementSpeedData : IComponentData
    {
        public float Value;
    }
}
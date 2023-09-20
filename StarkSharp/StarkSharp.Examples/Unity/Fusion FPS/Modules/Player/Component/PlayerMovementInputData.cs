using Unity.Entities;
using Unity.Mathematics;

namespace ECS
{
    public struct PlayerMovementInputData : IComponentData
    {
        public float2 Value;
    }
}

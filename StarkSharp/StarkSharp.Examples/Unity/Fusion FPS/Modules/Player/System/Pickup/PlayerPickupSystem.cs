using Unity.Burst;
using Unity.Entities;
namespace ECS
{
    [BurstCompile]
    public partial struct PlayerPickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Create a new Player Pickup Job and Schedule
            new PlayerPickupJob
            {

            }.Schedule();
        }
    }
}

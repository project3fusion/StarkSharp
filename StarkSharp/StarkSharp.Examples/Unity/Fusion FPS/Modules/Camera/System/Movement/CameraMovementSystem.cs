using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace ECS
{
    public partial struct CameraMovementSystem : ISystem
    {
        private Entity playerOwnerEntity;
        private Entity cameraEntity;
        private Player player;

        public void OnUpdate(ref SystemState state)
        {
            //Get player owner entity
            playerOwnerEntity = SystemAPI.GetSingletonEntity<PlayerOwnerTag>();

            //Get player aspect from player owner entity
            player = SystemAPI.GetAspect<Player>(playerOwnerEntity);

            //Create a new entity command buffer and allocate memory
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            //Get camera entity from camera singleton with camera tag
            cameraEntity = SystemAPI.GetSingletonEntity<CameraTag>();
            
            //Use entity command buffer to update the transform of the camera
            ecb.SetComponent(cameraEntity, LocalTransform.FromPositionRotation(
                player.playerLocalTransform.Position,
                player.playerLocalTransform.Rotation
            ));

            //Play entity command buffer
            ecb.Playback(state.EntityManager);

            //Dispose entity command buffer, free memory
            ecb.Dispose();

            //Set global variable camera entity to camera entity local variable
            Global.Variables.cameraEntity = cameraEntity;

            /*
            //Create a new player camera movement job and schedule
            new CameraMovementJob
            {
                deltaTime = deltaTime,
                playerPosition = player.playerLocalTransform.Position,
                playerRotation = player.playerLocalTransform.Rotation
            }.Schedule();
            */
        }
    }
}

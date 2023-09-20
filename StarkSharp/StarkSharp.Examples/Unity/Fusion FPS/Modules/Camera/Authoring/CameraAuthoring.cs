using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    public class CameraAuthoring : MonoBehaviour
    {
        public float cameraMovementSpeed;

        public class CameraAuthoringBaker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                //Get camera entity
                var cameraEntity = GetEntity(TransformUsageFlags.Dynamic);

                //Add Tags
                AddComponent<CameraTag>(cameraEntity);

                //Add Data
                AddComponent(cameraEntity, new PlayerCameraMovementSpeedData
                {
                    Value = 2
                });
            }
        }
    }
}
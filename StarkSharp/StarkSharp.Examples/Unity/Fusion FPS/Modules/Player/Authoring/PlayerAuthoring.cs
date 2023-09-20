using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public class PlayerAuthoring : MonoBehaviour
    {
        //Predefined Attributes
        public float playerMoveSpeed;
        public GameObject projectilePrefab;

        public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                //Get Player Entity
                var playerEntity = GetEntity(TransformUsageFlags.Dynamic);

                //Add Tags
                AddComponent<PlayerTag>(playerEntity);
                AddComponent<PlayerOwnerTag>(playerEntity);
                AddComponent<TargeterTag>(playerEntity);
                AddComponent<TargetTag>(playerEntity);

                //Add Data
                AddComponent<PlayerMovementInputData>(playerEntity);
                AddComponent<PlayerRotationInputData>(playerEntity);
                AddComponent(playerEntity, new PlayerMovementSpeedData
                {
                    Value = authoring.playerMoveSpeed
                });
                AddComponent(playerEntity, new PlayerRotationSpeedData
                {
                    Value = authoring.playerMoveSpeed
                });
                AddComponent(playerEntity, new ProjectilePrefabData
                {
                    Value = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
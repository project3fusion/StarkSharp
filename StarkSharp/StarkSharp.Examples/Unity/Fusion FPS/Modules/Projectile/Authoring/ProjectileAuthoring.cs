using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public float projectileMovementSpeed;

        public class ProjectileAuthoringBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                //Get Projectile Entity
                var projectileEntity = GetEntity(TransformUsageFlags.Dynamic);

                //Add Tags
                AddComponent<ProjectileTag>(projectileEntity);

                //Add Data
                AddComponent(projectileEntity, new ProjectileMovementSpeedData
                {
                    Value = authoring.projectileMovementSpeed
                });
            }
        }
    }
}
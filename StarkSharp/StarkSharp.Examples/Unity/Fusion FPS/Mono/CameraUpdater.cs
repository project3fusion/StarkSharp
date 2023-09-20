using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraUpdater : MonoBehaviour
{
    public float3 offset;
    private EntityManager entityManager;
    private LocalToWorld cameraLocalToWorld;

    private void Awake() => entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    private void LateUpdate()
    {
        if (Global.Variables.cameraEntity == null) return;
        cameraLocalToWorld = entityManager.GetComponentData<LocalToWorld>(Global.Variables.cameraEntity);
        transform.position = cameraLocalToWorld.Position + offset;
        transform.rotation = cameraLocalToWorld.Rotation;
    }
}

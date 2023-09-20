using Unity.Entities;
using Unity.Transforms;

namespace ECS
{
    public readonly partial struct Camera : IAspect
    {
        public readonly RefRO<CameraPositionData> cameraPositionData;
    }
}

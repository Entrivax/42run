using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class ObstacleTrash : Obstacle
    {
        public static Object3D MeshToUse;

        public ObstacleTrash(AxisAlignedBB boundingBox, Vector3 position, Direction direction) : base(boundingBox, position, direction)
        {
            Mesh = MeshToUse;
        }
    }
}

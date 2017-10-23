using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class Ground
    {
        public Object3D Mesh;
        public AxisAlignedBB BoundingBox;
        public Vector3 Position;
        public float Length;
        public Direction Direction;
        public Vector3[] PossibleObstaclePositions = new Vector3[0];
    }
}

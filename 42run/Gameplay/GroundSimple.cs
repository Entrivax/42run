using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class GroundSimple : Ground
    {
        public static Mesh MeshToUse;

        public GroundSimple(Direction direction)
        {
            Mesh = MeshToUse;
            Direction = direction;
            Length = 6f;
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector4(-3f, -0.5f, -Length, 1) * rotation;
            var p2 = new Vector4(3f, 0f, 0f, 1) * rotation;
            var p1v3 = new Vector3(p1);
            var p2v3 = new Vector3(p2);
            BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1v3, p2v3), Vector3.ComponentMax(p1v3, p2v3));
        }
    }
}

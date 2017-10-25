using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class GroundSimple : Ground
    {
        public static Object3D MeshToUse;

        public GroundSimple(Direction direction)
        {
            Mesh = MeshToUse;
            Direction = direction;
            Length = 6f;
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector4(-3f, -0.5f, -Length, 1) * rotation;
            var p2 = new Vector4(3f, 0f, 0f, 1) * rotation;
            PossibleObstaclePositions = new Vector3[2];
            for (int i = 0; i < 2; i++)
            {
                PossibleObstaclePositions[i] = (new Vector4(0, 0, -i * 3, 1) * rotation).Xyz;
            }
            BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1.Xyz, p2.Xyz), Vector3.ComponentMax(p1.Xyz, p2.Xyz));
        }
    }
}

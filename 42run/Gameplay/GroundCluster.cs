using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class GroundCluster : Ground
    {
        public static Object3D MeshToUse;

        public GroundCluster(Direction direction)
        {
            Mesh = MeshToUse;
            Direction = direction;
            Length = 32f;
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector4(-3f, -0.5f, -Length, 1) * rotation;
            var p2 = new Vector4(3f, 0f, 0f, 1) * rotation;
            PossibleObstaclePositions = new Vector3[3];
            for (int i = 0; i < 3; i++)
            {
                PossibleObstaclePositions[i] = (new Vector4(0, 0, -(i + 1) * 8, 1) * rotation).Xyz;
            }
            BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1.Xyz, p2.Xyz), Vector3.ComponentMax(p1.Xyz, p2.Xyz));
        }
    }
}

using OpenTK;

namespace _42run.Gameplay
{
    public static class ObstacleFactory
    {
        public static Obstacle NewObstacle(Vector3 position, Direction direction)
        {
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector3(new Vector4(-0.4f, 0f, -0.4f, 1) * rotation);
            var p2 = new Vector3(new Vector4(0.4f, 1.2f, 0.4f, 1) * rotation);
            return new ObstacleTrash(new AxisAlignedBB(Vector3.ComponentMin(p1, p2), Vector3.ComponentMax(p1, p2)), position, direction);
        }
    }
}

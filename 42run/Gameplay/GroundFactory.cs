using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public static class GroundFactory
    {
        public static Mesh GroundMesh { get; set; }

        public static Ground NewGround(Vector3 position, Direction direction)
        {
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector4(-3f, -0.5f, -3f, 1) * rotation;
            var p2 = new Vector4(3f, 0f, 3f, 1) * rotation;
            var p1v3 = new Vector3(p1);
            var p2v3 = new Vector3(p2);
            return new Ground { BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1v3, p2v3), Vector3.ComponentMax(p1v3, p2v3)), Mesh = GroundMesh, Position = position, Direction = direction };
        }

        public static Intersection NewIntersection(Player player, World world, Vector3 position, Direction direction, int directions)
        {
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector4(-3f, -0.5f, -2f, 1) * rotation;
            var p2 = new Vector4(3f, 0f, 2f, 1) * rotation;
            var p1v3 = new Vector3(p1);
            var p2v3 = new Vector3(p2);
            return new Intersection(player, world, position, direction) { BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1v3, p2v3), Vector3.ComponentMax(p1v3, p2v3)), Mesh = GroundMesh, ActivableBoundingBox = new AxisAlignedBB(new Vector3(-3f, 0f, -3f), new Vector3(3f, 3f, 3f)), Directions = directions, Used = false };
        }
    }
}

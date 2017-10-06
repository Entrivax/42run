using OpenTK;

namespace _42run.Gameplay
{
    public class Intersection : Ground
    {
        public AxisAlignedBB ActivableBoundingBox { get; set; }
        public int Directions { get; set; }

        public bool Used { get; set; } = false;

        public enum IntersectionDirection : int
        {
            LEFT = 1,
            RIGHT = 1 << 1
        }

        public Intersection(Player player, World world, Vector3 position, Direction direction)
        {
            Position = position;
            Direction = direction;
            var rot = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector3(new Vector4(-2f, 0f, -2f, 1) * rot);
            var p2 = new Vector3(new Vector4(2f, 5f, 2f, 1) * rot);

            world.TriggersToAdd.Add(new TerrainGenerator(player, this) { Position = Position + -DirectionHelper.GetVectorFromDirection(direction) * 8f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1, p2), Vector3.ComponentMax(p1, p2)) });
        }
    }
}

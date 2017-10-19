using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class Obstacle
    {
        public Object3D Mesh { get; set; }
        public AxisAlignedBB BoundingBox { get; set; }
        public Vector3 Position { get; set; }
        public Direction Direction;

        public Obstacle(AxisAlignedBB boundingBox, Vector3 position, Direction direction)
        {
            BoundingBox = boundingBox;
            Position = position;
            Direction = direction;
        }
    }
}

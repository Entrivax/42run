using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class Obstacle
    {
        public Mesh Mesh { get; set; }
        public AxisAlignedBB BoundingBox { get; set; }
        public Vector3 Position { get; set; }

        public Obstacle(Mesh mesh, AxisAlignedBB boundingBox, Vector3 position)
        {
            Mesh = mesh;
            BoundingBox = boundingBox;
            Position = position;
        }
    }
}

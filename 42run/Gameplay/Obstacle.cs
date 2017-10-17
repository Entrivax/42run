using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class Obstacle
    {
        public Object3D Mesh { get; set; }
        public AxisAlignedBB BoundingBox { get; set; }
        public Vector3 Position { get; set; }

        public Obstacle(Object3D mesh, AxisAlignedBB boundingBox, Vector3 position)
        {
            Mesh = mesh;
            BoundingBox = boundingBox;
            Position = position;
        }
    }
}

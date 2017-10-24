using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class Coin
    {
        public static Object3D MeshToUse;
        public static AxisAlignedBB BoundingBox { get; } = new AxisAlignedBB(new Vector3(-1, 0, -1), new Vector3(1, 2, 1));

        public Object3D Mesh { get; private set; }
        public Vector3 Position { get; set; }

        public Coin(Vector3 position)
        {
            Position = position;
            Mesh = MeshToUse;
        }
    }
}

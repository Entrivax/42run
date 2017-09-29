using _42run.OpenGL;
using OpenTK;

namespace _42run.Gameplay
{
    public class AxisAlignedBB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        private static Mesh _mesh;

        public AxisAlignedBB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public AxisAlignedBB(float minx, float miny, float minz, float maxx, float maxy, float maxz)
        {
            Min = new Vector3(minx, miny, minz);
            Max = new Vector3(maxx, maxy, maxz);
        }

        public bool IntersectWith(Vector3 thisOffset, AxisAlignedBB other, Vector3 otherOffset)
        {
            var thisOffsetted = new AxisAlignedBB(Min + thisOffset, Max + thisOffset);
            var otherOffsetted = new AxisAlignedBB(other.Min + otherOffset, other.Max + otherOffset);
            return thisOffsetted.IntersectWith(otherOffsetted);
        }

        public bool IntersectWith(AxisAlignedBB other)
        {
            if (other == null)
                return false;
            if (other.Max.X <= Min.X || other.Min.X >= Max.X)
                return false;
            if (other.Max.Y <= Min.Y || other.Min.Y >= Max.Y)
                return false;
            return other.Max.Z > Min.Z && other.Min.Z < Max.Z;
        }

        public static void SetMesh(Mesh mesh)
        {
            _mesh = mesh;
        }

        public Matrix4 CreateModelMatrix()
        {
            return Matrix4.CreateScale(Max - Min) * Matrix4.CreateTranslation((Max + Min) / 2);
        }

        public void Draw()
        {
            _mesh.Draw(OpenTK.Graphics.OpenGL4.PolygonMode.Line);
        }
    }
}

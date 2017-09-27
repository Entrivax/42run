using OpenTK;

namespace _42run.Gameplay
{
    public class AxisAlignedBB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

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

        public bool IntersectWith(AxisAlignedBB other)
        {
            if (other.Max.X <= Min.X || other.Min.X >= Max.X)
            {
                return false;
            }
            if (other.Max.Y <= Min.Y || other.Min.Y >= Max.Y)
            {
                return false;
            }
            return other.Max.Z > Min.Z && other.Min.Z < Max.Z;
        }
    }
}

using OpenTK;

namespace _42run.Gameplay
{
    public abstract class Trigger
    {
        public AxisAlignedBB BoundingBox { get; set; }
        public Vector3 Position { get; set; }

        public abstract bool Update();
    }
}

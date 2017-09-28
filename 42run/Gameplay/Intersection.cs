namespace _42run.Gameplay
{
    public class Intersection : Ground
    {
        public AxisAlignedBB ActivableBoundingBox { get; set; }
        public int Directions { get; set; }

        public bool Used { get; set; }

        enum Direction : int
        {
            FORWARD,
            LEFT,
            RIGHT
        }
    }
}

using OpenTK;

namespace _42run.Gameplay
{
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST,
    }

    public class DirectionHelper
    {
        public static Vector3 GetVectorFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.WEST:
                    return new Vector3(-1, 0, 0);
                case Direction.SOUTH:
                    return new Vector3(0, 0, 1);
                case Direction.EAST:
                    return new Vector3(1, 0, 0);
                case Direction.NORTH:
                default:
                    return new Vector3(0, 0, -1);
            }
        }

        public static Matrix4 GetRotationFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.WEST:
                    return Matrix4.CreateRotationY(MathHelper.PiOver2);
                case Direction.EAST:
                    return Matrix4.CreateRotationY(-MathHelper.PiOver2);
                case Direction.SOUTH:
                    return Matrix4.CreateRotationY(MathHelper.Pi);
                case Direction.NORTH:
                default:
                    return Matrix4.Identity;
            }
        }
    }
}

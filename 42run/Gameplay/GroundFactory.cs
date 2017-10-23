using _42run.OpenGL;
using OpenTK;
using System;

namespace _42run.Gameplay
{
    public static class GroundFactory
    {
        private static Random _random = new Random();

        public static Ground[] NewGround(Vector3 position, Direction direction, out Vector3 next)
        {
            var groundNum = _random.Next(0, 100);
            Ground[] grounds;
            if (groundNum < 80)
            {
                var ground = new GroundSimple(direction) { Position = position };
                grounds = new[] { ground };
                next = position + DirectionHelper.GetVectorFromDirection(direction) * ground.Length;
            }
            else if (groundNum < 90)
            {
                var ground = new GroundCluster(direction) { Position = position };
                grounds = new[] { ground };
                next = position + DirectionHelper.GetVectorFromDirection(direction) * ground.Length;
            }
            else
            {
                var groundCount = _random.Next(5, 10);
                grounds = new Ground[groundCount];
                next = position;
                for (int i = 0; i < groundCount; i++)
                {
                    var ground = new GroundStairs(direction) { Position = next };
                    grounds[i] = ground;
                    next = next + DirectionHelper.GetVectorFromDirection(direction) * ground.Length + new Vector3(0, 0.3f, 0);
                }
            }
            return grounds;
        }

        public static Intersection NewIntersection(Player player, World world, Vector3 position, Direction direction, int directions)
        {
            var rotation = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector3(new Vector4(-6f, -0.5f, -6f, 1) * rotation);
            var p2 = new Vector3(new Vector4(6f, 0f, 0f, 1) * rotation);
            var ap1 = new Vector3(new Vector4(-3f, 0f, -6f, 1) * rotation);
            var ap2 = new Vector3(new Vector4(3f, 5f, 0f, 1) * rotation);
            return new Intersection(player, world, position, direction) { BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1, p2), Vector3.ComponentMax(p1, p2)), ActivableBoundingBox = new AxisAlignedBB(Vector3.ComponentMin(ap1, ap2), Vector3.ComponentMax(ap1, ap2)), Directions = directions, Used = false };
        }
    }
}

using _42run.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;

namespace _42run.Gameplay
{
    public class Intersection : Ground
    {
        public AxisAlignedBB ActivableBoundingBox { get; set; }

        private int _directions;

        public int Directions { get { return _directions; } set { _directions = value; if (_directions == (int)IntersectionDirection.LEFT) Mesh = Right_Mesh; else if (_directions == (int)IntersectionDirection.RIGHT) Mesh = Left_Mesh; else if (_directions == ((int)IntersectionDirection.LEFT | (int)IntersectionDirection.RIGHT)) Mesh = LeftRight_Mesh; } }

        public static Mesh Left_Mesh { get; set; }
        public static Mesh Right_Mesh { get; set; }
        public static Mesh LeftRight_Mesh { get; set; }

        public bool Used { get; set; } = false;

        public enum IntersectionDirection : int
        {
            LEFT = 1,
            RIGHT = 1 << 1
        }

        public Intersection(Player player, World world, Vector3 position, Direction direction)
        {
            Position = position;
            Direction = direction;
            Length = 6f;
            var rot = DirectionHelper.GetRotationFromDirection(direction);
            var p1 = new Vector3(new Vector4(-3f, 0f, 0f, 1) * rot);
            var p2 = new Vector3(new Vector4(3f, 5f, -6f, 1) * rot);

            world.TriggersToAdd.Add(new TerrainGenerator(player, this) { Position = Position + -DirectionHelper.GetVectorFromDirection(direction) * 8f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(p1, p2), Vector3.ComponentMax(p1, p2)) });
        }
    }
}

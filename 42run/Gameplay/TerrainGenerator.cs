using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _42run.Gameplay
{
    public class TerrainGenerator : Trigger
    {
        private Player _player;
        private Intersection _intersection;
        private Random _rand;

        public TerrainGenerator(Player player, Intersection intersection)
        {
            _player = player;
            _intersection = intersection;
            _rand = new Random();
        }

        public override bool Update()
        {
            var playerPos = _player.GetPosition();
            if (BoundingBox.IntersectWith(Position, _player.BoundingBox, playerPos))
            {
                GenerateTerrain();
                return true;
            }
            return false;
        }

        private void GenerateTerrain()
        {
            TerrainRemover leftTerrainRemover = null;
            if((_intersection.Directions & (int)Intersection.IntersectionDirection.LEFT) > 0)
            {
                var dir = (int)_intersection.Direction - 1;
                if (dir < 0)
                    dir = (int)Direction.WEST;
                var direction = (Direction)dir;
                int toGenerate = _rand.Next(10) + 5;
                List<Ground> generatedGrounds = new List<Ground>();
                var dirVector = DirectionHelper.GetVectorFromDirection(direction);
                var nextPosition = _intersection.Position + dirVector * 5f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f;
                for (int i = 0; i < toGenerate; i++)
                {
                    generatedGrounds.Add(GroundFactory.NewGround(nextPosition, direction, out nextPosition));
                }
                var intersection = GroundFactory.NewIntersection(_player, _player.World, nextPosition, direction, _rand.Next(1, 4));
                generatedGrounds.Add(intersection);
                _player.World.Grounds.AddRange(generatedGrounds);

                var rotation = DirectionHelper.GetRotationFromDirection(direction);
                var ap1 = new Vector3(new Vector4(-3f, 0f, -6f, 1) * rotation);
                var ap2 = new Vector3(new Vector4(3f, 5f, 0f, 1) * rotation);
                leftTerrainRemover = new TerrainRemover(_player, generatedGrounds, new List<Trigger>(_player.World.TriggersToAdd.ToArray())) { Position = _intersection.Position + dirVector * 10f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(ap1, ap2), Vector3.ComponentMax(ap1, ap2)) };
                _player.World.TriggersToAdd.Add(leftTerrainRemover);
            }
            if ((_intersection.Directions & (int)Intersection.IntersectionDirection.RIGHT) > 0)
            {
                var dir = (int)_intersection.Direction + 1;
                if (dir > 3)
                    dir = (int)Direction.NORTH;
                var direction = (Direction)dir;
                int toGenerate = _rand.Next(10) + 5;
                List<Ground> generatedGrounds = new List<Ground>();
                var dirVector = DirectionHelper.GetVectorFromDirection(direction);
                var nextPosition = _intersection.Position + dirVector * 5f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f;
                for (int i = 0; i < toGenerate; i++)
                {
                    generatedGrounds.Add(GroundFactory.NewGround(nextPosition, direction, out nextPosition));
                }
                var intersection = GroundFactory.NewIntersection(_player, _player.World, nextPosition, direction, _rand.Next(1, 4));
                generatedGrounds.Add(intersection);
                _player.World.Grounds.AddRange(generatedGrounds);

                var triggersToKeep = new List<Trigger>(_player.World.TriggersToAdd.ToArray());

                if (leftTerrainRemover != null && triggersToKeep.Contains(leftTerrainRemover))
                    triggersToKeep.Remove(leftTerrainRemover);

                var rotation = DirectionHelper.GetRotationFromDirection(direction);
                var ap1 = new Vector3(new Vector4(-3f, 0f, -6f, 1) * rotation);
                var ap2 = new Vector3(new Vector4(3f, 5f, 0f, 1) * rotation);
                var terrainRemover = new TerrainRemover(_player, generatedGrounds, triggersToKeep) { Position = _intersection.Position + dirVector * 10f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(ap1, ap2), Vector3.ComponentMax(ap1, ap2)) };
                _player.World.TriggersToAdd.Add(terrainRemover);
            }
        }
    }
}

﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var coinsOffset = new Vector3(0, 0.3f, 0);
            if((_intersection.Directions & (int)Intersection.IntersectionDirection.LEFT) > 0)
            {
                var dir = (int)_intersection.Direction - 1;
                if (dir < 0)
                    dir = (int)Direction.WEST;
                var direction = (Direction)dir;
                int toGenerate = _rand.Next(10) + 5;
                List<Ground> generatedGrounds = new List<Ground>();
                var dirVector = DirectionHelper.GetVectorFromDirection(direction);
                var interDir = DirectionHelper.GetVectorFromDirection(_intersection.Direction);
                var nextPosition = _intersection.Position + dirVector * 5f + interDir * 3f;
                List<Vector3> obstaclePositions = new List<Vector3>();
                for (int i = 0; i < toGenerate; i++)
                {
                    var grounds = GroundFactory.NewGround(nextPosition, direction, out nextPosition);
                    Array.ForEach(grounds, ground => Array.ForEach(ground.PossibleObstaclePositions, possiblePos => obstaclePositions.Add(possiblePos + ground.Position)));
                    generatedGrounds.AddRange(grounds);
                }
                var intersection = GroundFactory.NewIntersection(_player, _player.World, nextPosition, direction, _rand.Next(1, 4));
                generatedGrounds.Add(intersection);
                _player.World.Grounds.AddRange(generatedGrounds);

                var rotation = DirectionHelper.GetRotationFromDirection(direction);
                var wallsColliders = new List<Obstacle>();
                var coins = new List<Coin>();

                obstaclePositions.ForEach(pos => {
                    if ((pos - _intersection.Position).LengthFast > 20)
                    {
                        var randValue = _rand.Next(4);
                        if (randValue == 0)
                            wallsColliders.Add(ObstacleFactory.NewObstacle(pos + interDir * _rand.Next(-1, 1) * 2, direction));
                        else if (randValue == 1)
                            coins.Add(new Coin(pos + interDir * _rand.Next(-1, 1) * 2 + coinsOffset));
                    }
                });
                _player.World.Coins.AddRange(coins);

                var w1p1 = new Vector3(new Vector4(-3f, 0f, -7f, 1) * rotation);
                var w1p2 = new Vector3(new Vector4(3f, 5f, -6f, 1) * rotation);
                wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w1p1, w1p2), Vector3.ComponentMax(w1p1, w1p2)), intersection.Position, Direction.NORTH));
                if (intersection.Directions == (int)Intersection.IntersectionDirection.LEFT)
                {
                    var w2p1 = new Vector3(new Vector4(3f, 0f, -6f, 1) * rotation);
                    var w2p2 = new Vector3(new Vector4(4f, 5f, 0f, 1) * rotation);
                    wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position, Direction.NORTH));
                }
                if (intersection.Directions == (int)Intersection.IntersectionDirection.RIGHT)
                {
                    var w2p1 = new Vector3(new Vector4(-4f, 0f, -6f, 1) * rotation);
                    var w2p2 = new Vector3(new Vector4(-3f, 5f, 0f, 1) * rotation);
                    wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position, Direction.NORTH));
                }
                _player.World.Obstacles.AddRange(wallsColliders);
                var ap1 = new Vector3(new Vector4(-3f, 0f, -6f, 1) * rotation);
                var ap2 = new Vector3(new Vector4(3f, 5f, 0f, 1) * rotation);
                leftTerrainRemover = new TerrainRemover(_player, generatedGrounds, new List<Trigger>(_player.World.TriggersToAdd.ToArray()), wallsColliders, coins) { Position = _intersection.Position + dirVector * 12f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(ap1, ap2), Vector3.ComponentMax(ap1, ap2)) };
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
                var interDir = DirectionHelper.GetVectorFromDirection(_intersection.Direction);
                var nextPosition = _intersection.Position + dirVector * 5f + interDir * 3f;
                List<Vector3> obstaclePositions = new List<Vector3>();
                for (int i = 0; i < toGenerate; i++)
                {
                    var grounds = GroundFactory.NewGround(nextPosition, direction, out nextPosition);
                    Array.ForEach(grounds, ground => Array.ForEach(ground.PossibleObstaclePositions, possiblePos => obstaclePositions.Add(possiblePos + ground.Position)));
                    generatedGrounds.AddRange(grounds);
                }
                var intersection = GroundFactory.NewIntersection(_player, _player.World, nextPosition, direction, _rand.Next(1, 4));
                generatedGrounds.Add(intersection);
                _player.World.Grounds.AddRange(generatedGrounds);

                var triggersToKeep = new List<Trigger>(_player.World.TriggersToAdd.ToArray());

                if (leftTerrainRemover != null && triggersToKeep.Contains(leftTerrainRemover))
                    triggersToKeep.Remove(leftTerrainRemover);

                var rotation = DirectionHelper.GetRotationFromDirection(direction);
                var wallsColliders = new List<Obstacle>();
                var coins = new List<Coin>();

                obstaclePositions.ForEach(pos => {
                    if ((pos - _intersection.Position).LengthFast > 20)
                    {
                        var randValue = _rand.Next(4);
                        if (randValue == 0)
                            wallsColliders.Add(ObstacleFactory.NewObstacle(pos + interDir * _rand.Next(-1, 1) * 2, direction));
                        else if (randValue == 1)
                            coins.Add(new Coin(pos + interDir * _rand.Next(-1, 1) * 2 + coinsOffset));
                    }
                });
                _player.World.Coins.AddRange(coins);

                var w1p1 = new Vector3(new Vector4(-3f, 0f, -7f, 1) * rotation);
                var w1p2 = new Vector3(new Vector4(3f, 5f, -6f, 1) * rotation);
                wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w1p1, w1p2), Vector3.ComponentMax(w1p1, w1p2)), intersection.Position, Direction.NORTH));
                if (intersection.Directions == (int)Intersection.IntersectionDirection.LEFT)
                {
                    var w2p1 = new Vector3(new Vector4(3f, 0f, -6f, 1) * rotation);
                    var w2p2 = new Vector3(new Vector4(4f, 5f, 0f, 1) * rotation);
                    wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position, Direction.NORTH));
                }
                if (intersection.Directions == (int)Intersection.IntersectionDirection.RIGHT)
                {
                    var w2p1 = new Vector3(new Vector4(-4f, 0f, -6f, 1) * rotation);
                    var w2p2 = new Vector3(new Vector4(-3f, 5f, 0f, 1) * rotation);
                    wallsColliders.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position, Direction.NORTH));
                }
                _player.World.Obstacles.AddRange(wallsColliders);
                var ap1 = new Vector3(new Vector4(-3f, 0f, -6f, 1) * rotation);
                var ap2 = new Vector3(new Vector4(3f, 5f, 0f, 1) * rotation);
                var terrainRemover = new TerrainRemover(_player, generatedGrounds, triggersToKeep, wallsColliders, coins) { Position = _intersection.Position + dirVector * 12f + DirectionHelper.GetVectorFromDirection(_intersection.Direction) * 3f, BoundingBox = new AxisAlignedBB(Vector3.ComponentMin(ap1, ap2), Vector3.ComponentMax(ap1, ap2)) };
                _player.World.TriggersToAdd.Add(terrainRemover);
            }
        }
    }
}

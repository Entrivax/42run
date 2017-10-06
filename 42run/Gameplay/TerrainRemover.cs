using System.Collections.Generic;

namespace _42run.Gameplay
{
    public class TerrainRemover : Trigger
    {
        private List<Ground> _groundsToKeep;
        private List<Trigger> _triggersToKeep;
        private Player _player;

        public TerrainRemover(Player player, List<Ground> groundsToKeep, List<Trigger> triggersToKeep)
        {
            _groundsToKeep = groundsToKeep;
            _triggersToKeep = triggersToKeep;
            _player = player;
        }

        public override bool Update()
        {
            var playerPos = _player.GetPosition();
            if (BoundingBox.IntersectWith(Position, _player.BoundingBox, playerPos))
            {
                DestroyTerrain();
                return true;
            }
            return false;
        }

        private void DestroyTerrain()
        {
            _player.World.Grounds.Clear();
            _player.World.Grounds.AddRange(_groundsToKeep);
            _player.World.TriggersToRemove.AddRange(_player.World.Triggers);
            _player.World.TriggersToAdd.AddRange(_triggersToKeep);
        }
    }
}

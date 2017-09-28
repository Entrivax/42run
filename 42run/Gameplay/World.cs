using System.Collections.Generic;

namespace _42run.Gameplay
{
    public class World
    {
        public List<Obstacle> Obstacles { get; private set; }
        public List<Ground> Grounds { get; private set; }
    }
}
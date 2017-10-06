using System.Collections.Generic;

namespace _42run.Gameplay
{
    public class World
    {
        public List<Obstacle> Obstacles { get; private set; }
        public List<Ground> Grounds { get; private set; }
        public List<Trigger> Triggers { get; private set; }
        public List<Trigger> TriggersToAdd { get; private set; }
        public List<Trigger> TriggersToRemove { get; private set; }

        public World()
        {
            Obstacles = new List<Obstacle>();
            Grounds = new List<Ground>();
            Triggers = new List<Trigger>();
            TriggersToAdd = new List<Trigger>();
            TriggersToRemove = new List<Trigger>();
        }

        public void Update()
        {
            List<Trigger> triggersToRemove = new List<Trigger>();
            foreach (var trigger in Triggers)
            {
                if (trigger.Update())
                    triggersToRemove.Add(trigger);
            }
            foreach (var triggerToRemove in triggersToRemove)
                Triggers.Remove(triggerToRemove);
            foreach (var triggerToRemove in TriggersToRemove)
                Triggers.Remove(triggerToRemove);
            foreach (var trigger in TriggersToAdd)
                Triggers.Add(trigger);
            TriggersToAdd.Clear();
            TriggersToRemove.Clear();
        }
    }
}
using OpenTK;
using System;

namespace _42run.Gameplay
{
    public class Player : ICollidable
    {
        public Vector3 Direction;
        public Vector3 Position;
        public float Speed;

        public void Update(double time)
        {
            Position += Direction * Speed * (float)time;
        }

        public void OnCollide(ICollidable with)
        {
            throw new NotImplementedException();
        }
    }
}

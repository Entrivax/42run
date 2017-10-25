using _42run.OpenGL;
using OpenTK;
using System;

namespace _42run.Gameplay
{
    public class CloneableCoin : ICloneable
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation => Quaternion.FromEulerAngles(_rotations);

        public Vector3 RotationVelocity { get; set; }
        public Vector3 Velocity { get; set; }

        private static readonly Vector3 _gravity = new Vector3(0, 900f, 0);

        public float LifeTime { get; set; }

        private Vector3 _rotations;

        public Object3D Mesh { get; set; }

        public CloneableCoin(Object3D mesh)
        {
            Mesh = mesh;
        }

        public void Update(double time)
        {
            Velocity -= _gravity * (float)time;
            _rotations += RotationVelocity * (float)time;
            Position += Velocity * (float)time;
            LifeTime += (float)time;
        }

        public void SetRotations(Vector3 rotations)
        {
            _rotations = rotations;
        }

        public object Clone()
        {
            return new CloneableCoin(Mesh)
            {
                _rotations = _rotations,
                Velocity = Velocity,
                RotationVelocity = RotationVelocity,
                Position = Position
            };
        }
    }
}

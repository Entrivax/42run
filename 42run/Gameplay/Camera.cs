﻿using OpenTK;

namespace _42run.Gameplay
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public float Fov { get; set; }

        public Camera(Vector3 position, Vector3 target, float fov)
        {
            Position = position;
            Target = target;
            Fov = fov;
        }

        public void UpdateCameraPosition(Vector3 toPosition, float delta, float speed)
        {
            Position = Vector3.Lerp(Position, toPosition, delta * speed);
        }

        public Matrix4 ComputeProjectionMatrix(float ratio)
        {
            return Matrix4.CreatePerspectiveFieldOfView(Fov, ratio, 0.1f, 100f);
        }

        public Matrix4 ComputeViewMatrix()
        {
            return Matrix4.LookAt(Position, Target, Vector3.UnitY);
        }
    }
}

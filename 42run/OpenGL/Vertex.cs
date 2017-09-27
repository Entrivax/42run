using OpenTK;

namespace _42run.OpenGL
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 Uv;

        public Vertex(Vector3 position, Vector2 uv)
        {
            Position = position;
            Uv = uv;
        }
    }
}

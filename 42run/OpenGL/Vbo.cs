using OpenTK.Graphics.OpenGL4;
using System;

namespace _42run
{
    public class Vbo<T> : IDisposable where T : struct
    {
        public int Buffer { get; private set; } = -1;

        public Vbo()
        {
            int buffer;
            GL.GenBuffers(1, out buffer);
            Buffer = buffer;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer);
        }

        public void SetData(T[] data)
        {
            GL.BufferData<T>(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * sizeof(T)), T, BufferUsageHint.StaticDraw);
        }

        public void Dispose()
        {
            if (Buffer != -1)
                GL.DeleteBuffers(1, new []{ Buffer });
        }
    }
}


using OpenTK.Graphics.OpenGL4;
using System;

namespace _42run
{
    public class Vao<T> : IDisposable where T : struct
    {
        public int Array { get; private set; }

        public Vao()
        {
            int array;
            GL.GenVertexArrays(1, out array);
            Array = array;
        }

        public void BindVbo(Vbo<T> vbo)
        {
            GL.BindVertexArray(Array);
            vbo.Bind();
            // Set attributes

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}


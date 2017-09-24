using _42run.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System;

namespace _42run.OpenGL
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

        public void BindVbo(Vbo vbo, Shader shader, VertexAttribute[] attributes)
        {
            GL.BindVertexArray(Array);
            vbo.Bind();
            
            foreach(var attrib in attributes)
            {
                attrib.Set(shader);
            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}


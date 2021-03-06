﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace _42run.OpenGL
{
    public class Vbo : IDisposable
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

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetData(Vector3[] data)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * Vector3.SizeInBytes), data, BufferUsageHint.StaticDraw);
        }

        public void SetData(Vector2[] data)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * Vector2.SizeInBytes), data, BufferUsageHint.StaticDraw);
        }

        public void SetData(Vertex[] data)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * (Vector2.SizeInBytes + Vector3.SizeInBytes)), data, BufferUsageHint.StaticDraw);
        }

        public void Dispose()
        {
            if (Buffer != -1)
                GL.DeleteBuffers(1, new []{ Buffer });
            Buffer = -1;
        }
    }
}


using OpenTK.Graphics.OpenGL4;
using System;

namespace _42run.OpenGL
{
    public static class ShaderManager
    {
        public static void Use(Shader shader)
        {
            if (shader == null)
                throw new ArgumentNullException("Shader cannot be null");
            if (shader.ProgramId == -1)
                throw new ArgumentException("Shader is not loaded");
            GL.UseProgram(shader.ProgramId);
        }

        public static void Disable()
        {
            GL.UseProgram(0);
        }
    }
}

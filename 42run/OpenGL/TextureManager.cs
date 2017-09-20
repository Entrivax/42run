using OpenTK.Graphics.OpenGL4;

namespace _42run.OpenGL
{
    public static class TextureManager
    {
        public static void Use(Texture texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
        }

        public static void Disable()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}

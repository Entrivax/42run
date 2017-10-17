using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace _42run.OpenGL
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        public static Texture Get(string name)
        {
            if (_textures.ContainsKey(name))
                return _textures[name];
            var texture = new Texture(name);
            _textures.Add(name, texture);
            return texture;
        }

        public static void DisposeTexture(string name)
        {
            if (!_textures.ContainsKey(name))
                return;
            var texture = _textures[name];
            _textures.Remove(name);
            texture.Dispose();
        }

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

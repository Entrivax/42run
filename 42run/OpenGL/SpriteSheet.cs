using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace _42run.OpenGL
{
    public class SpriteSheet : IDisposable
    {
        public Shader Shader { get; private set; }
        public Texture Texture { get; private set; }
        public Object3D Mesh { get; private set; }

        private int _spriteWidth;
        private int _spriteHeight;
        private Vector2 _uv;

        public SpriteSheet(Object3D mesh, string path, int spriteWidth, int spriteHeight, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            Mesh = mesh;
            Shader = ShaderManager.Get("SpriteSheet");
            Texture = TextureManager.Get(path, minFilter, magFilter);
            _spriteWidth = spriteWidth;
            _spriteHeight = spriteHeight;
            _uv = new Vector2((float)_spriteWidth / Texture.Width, (float)_spriteHeight / Texture.Height);
        }

        public void Draw(int xSprite, int ySprite)
        {
            var uv = new Vector2(_uv.X * xSprite, _uv.Y * ySprite);
            Shader.SetUniform2("sprite", ref uv);
            TextureManager.Use(Texture);
            Mesh.Draw();
        }

        public void Dispose()
        {
            
        }
    }
}

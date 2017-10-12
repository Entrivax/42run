using System;

namespace _42run.OpenGL
{
    public class Font : IDisposable
    {
        private string _availableChars;

        public float CharLength { get; private set; }
        public float CharWidth { get; private set; }

        public Texture Texture { get; private set; }

        public Font(string path, string availableChars)
        {
            _availableChars = availableChars;
            CharLength = 1.0f / _availableChars.Length;
            Texture = new Texture(path);
            CharWidth = Texture.Width / _availableChars.Length;
        }

        public float GetUFor(char c)
        {
            return _availableChars.IndexOf(c) * CharLength;
        }

        public float GetStringWidth(string str)
        {
            return str.Length * CharWidth;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}

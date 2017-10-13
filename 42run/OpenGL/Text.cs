using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace _42run.OpenGL
{
    public class Text : IDisposable
    {
        public Vector2 Position { get; set; }
        public Font Font { get; set; }
        public Shader Shader { get; set; }
        public Alignment TextAlignment { get; set; } = Alignment.LEFT;

        private Vao<Vertex> _vao;
        private Vbo _vbo;

        public enum Alignment
        {
            LEFT,
            MIDDLE,
            RIGHT
        }

        public Text(Vector2 position, Font font, Shader shader, Alignment textAlignment, string str)
        {
            Position = position;
            Font = font;
            Shader = shader;
            TextAlignment = textAlignment;
            Str = str;
        }

        public Text(Vector2 position, Font font, Shader shader, string str) : this(position, font, shader, Alignment.LEFT, str) { }

        public void Dispose()
        {
            _vao?.Dispose();
            _vbo?.Dispose();
        }

        private string _str;
        public string Str
        {
            get { return _str; }
            set
            {
                _str = value;
                UpdateString();
            }
        }

        private void UpdateString()
        {
            var chars = new List<Vertex>(3 * _str.Length);
            
            var split = _str.Split(new[] { '\n' }, StringSplitOptions.None);
            var prevY = 0f;
            for (int i = 0; i < split.Length; i++)
            {
                var prevX = 0f;
                var leftPadding = TextAlignment == Alignment.LEFT ? 0 : (TextAlignment == Alignment.RIGHT ? -Font.GetStringWidth(split[i]) : -Font.GetStringWidth(split[i]) / 2);
                prevY += Font.Texture.Height;
                foreach (var c in split[i])
                {
                    var u = Font.GetUFor(c);
                    chars.AddRange(new Vertex[]
                    {
                        new Vertex(new Vector3(prevX + leftPadding, prevY + Font.Texture.Height, 0f), new Vector2(u, 0)),
                        new Vertex(new Vector3(prevX + leftPadding, prevY, 0f), new Vector2(u, 1)),
                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY + Font.Texture.Height, 0f), new Vector2(u + Font.CharLength, 0)),

                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY + Font.Texture.Height, 0f), new Vector2(u + Font.CharLength, 0)),
                        new Vertex(new Vector3(prevX + leftPadding, prevY, 0f), new Vector2(u, 1)),
                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY, 0f), new Vector2(u + Font.CharLength, 1)),
                    });

                    prevX += Font.CharWidth;
                }
            }
            
            if (_vbo == null)
                _vbo = new Vbo();
            _vbo.Bind();
            _vbo.SetData(chars.ToArray());
            _vbo.Unbind();

            if (_vao == null)
                _vao = new Vao<Vertex>();
            _vao.BindVbo(_vbo, Shader, new[] {
                new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, 0),
                new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, Vector3.SizeInBytes),
            });
        }

        public void Draw()
        {
            TextureManager.Use(Font.Texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _str.Length * 6);
            TextureManager.Disable();
        }
    }
}

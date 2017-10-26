using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace _42run.OpenGL
{
    public class Mesh : IDisposable
    {
        public List<Vertex> Vertices;

        public Material Material;

        private int VerticesCount = -1;
        private Vao<Vertex> _vao;
        private Vbo _vbo;

        public Mesh()
        {
            Vertices = new List<Vertex>();
        }

        public void Dispose()
        {
            ClearVertices();
            VerticesCount = 0;
            _vao.Dispose();
            _vbo.Dispose();
        }

        public void LoadInGl(Shader shader)
        {
            var vertices = Vertices.ToArray();
            _vbo = new Vbo();
            _vbo.Bind();
            _vbo.SetData(vertices);
            _vbo.Unbind();

            VerticesCount = vertices.Length;

            _vao = new Vao<Vertex>();
            _vao.BindVbo(_vbo, shader, new[] {
                new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, 0),
                new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, Vector3.SizeInBytes),
            });
            ClearVertices();
        }

        public void BindVao(Shader shader)
        {
        }

        public void ClearVertices()
        {
            Vertices = null;
        }

        public void Draw()
        {
            if (Material?.Texture != null)
                Draw(Material.Texture);
            else
            {
                _vao.Bind();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
            }
        }

        public void Draw(Texture texture)
        {
            TextureManager.Use(texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
            TextureManager.Disable();
        }

        public void Draw(PolygonMode mode)
        {
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, mode);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
        }
    }
}

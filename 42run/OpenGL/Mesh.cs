using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace _42run.OpenGL
{
    public class Mesh : IDisposable
    {
        public Vertex[] Vertices;

        private Vao<Vertex> _vao;
        private Vbo _vbo;

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
        }

        public void LoadFile(string path, bool invertX, bool invertY, bool invertZ)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<Vector3> points = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            using (var reader = File.OpenText(path))
            {
                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 4 && split[0] == "v")
                    {
                        points.Add(new Vector3(
                            float.Parse(split[1], CultureInfo.InvariantCulture) * (invertX ? -1 : 1),
                            float.Parse(split[2], CultureInfo.InvariantCulture) * (invertY ? -1 : 1),
                            float.Parse(split[3], CultureInfo.InvariantCulture) * (invertZ ? -1 : 1)));
                    }
                    if ((split.Length == 3 || split.Length == 4) && split[0] == "vt")
                    {
                        uvs.Add(new Vector2(
                            float.Parse(split[1], CultureInfo.InvariantCulture),
                            1 - float.Parse(split[2], CultureInfo.InvariantCulture)));
                    }
                    if (split.Length == 4 && split[0] == "f")
                    {
                        var vert1 = split[1].Split(new[] { '/' }, StringSplitOptions.None);
                        var vert2 = split[2].Split(new[] { '/' }, StringSplitOptions.None);
                        var vert3 = split[3].Split(new[] { '/' }, StringSplitOptions.None);

                        var uvIndex = int.Parse(vert1[1]) - 1;
                        vertices.Add(new Vertex(points[int.Parse(vert1[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                        uvIndex = int.Parse(vert2[1]) - 1;
                        vertices.Add(new Vertex(points[int.Parse(vert2[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                        uvIndex = int.Parse(vert3[1]) - 1;
                        vertices.Add(new Vertex(points[int.Parse(vert3[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                    }
                }
            }
            Vertices = vertices.ToArray();
        }

        public void LoadInGl(Shader shader)
        {
            _vbo = new Vbo();
            _vbo.Bind();
            _vbo.SetData(Vertices);
            _vbo.Unbind();
            
            _vao = new Vao<Vertex>();
            _vao.BindVbo(_vbo, shader, new[] {
                new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, 0),
                new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes, Vector3.SizeInBytes),
            });
        }

        public void Draw()
        {
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
        }

        public void Draw(Texture texture)
        {
            TextureManager.Use(texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            TextureManager.Disable();
        }

        public void Draw(PolygonMode mode)
        {
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, mode);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
        }
    }
}

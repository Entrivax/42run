using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using _42run.OpenGL;
using OpenTK;
using System.Collections.Generic;

namespace _42run.GameStates
{
    public class GameStateMenu : IGameState
    {
        private Color4 _backColor;
        private Font _font;
        private Shader _flatColorShader;
        private Shader _3dSpriteShader;
        private Text _text;
        private Matrix4 _guiProj;

        private Object3D _playerMesh;
        private SpriteSheet[] _playerSpriteSheets;

        private int _selected;

        private int _width;
        private int _height;
        private float _time;

        public GameStateMenu()
        {
            _backColor = new Color4(0f, 0f, 0f, 1f);
            _font = FontManager.Get("glyphs");
            _flatColorShader = ShaderManager.Get("FlatColorShader");
            _3dSpriteShader = ShaderManager.Get("SpriteSheet");
            _text = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.MIDDLE, $"Press the Enter button to start!");

            _selected = 0;
            
            var u = 24.0f / 240;
            _playerMesh = new Object3D(new[]
            {
                new Mesh
                {
                    Vertices = new List<Vertex>
                    {
                        new Vertex(new Vector3(-12, 32f, 0f), new Vector2(0, 0)),
                        new Vertex(new Vector3(-12, 0f, 0f), new Vector2(0, 1)),
                        new Vertex(new Vector3(12, 32f, 0f), new Vector2(u, 0)),

                        new Vertex(new Vector3(12, 32f, 0f), new Vector2(u, 0)),
                        new Vertex(new Vector3(-12, 0f, 0f), new Vector2(0, 1)),
                        new Vertex(new Vector3(12, 0f, 0f), new Vector2(u, 1)),
                    }
                }});
            _playerMesh.LoadInGl(_3dSpriteShader);
            _playerSpriteSheets = new[]
            {
                new SpriteSheet(_playerMesh, "running_link.png", 24, 32, TextureMinFilter.Nearest, TextureMagFilter.Nearest),
                new SpriteSheet(_playerMesh, "running_holo.png", 24, 32, TextureMinFilter.Nearest, TextureMagFilter.Nearest)
            };

        }

        public void Dispose()
        {
            _text?.Dispose();
            _text = null;
            foreach (var spriteSheet in _playerSpriteSheets)
            {
                spriteSheet.Dispose();
            }
            _playerSpriteSheets = null;
            _playerMesh?.Dispose();
            _playerMesh = null;
        }

        public void Draw(double deltaTime)
        {
            var textOffset = -new Vector2(_width / 2, _height / 2);
            GL.ClearColor(_backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            {
                _3dSpriteShader.Bind();

                _3dSpriteShader.SetUniformMatrix4("proj", false, ref _guiProj);

                for (int i = _selected > 0 ? _selected - 1 : 0; i < _selected + 1 || i < _playerSpriteSheets.Length; i++)
                {
                    var scale = _selected == i ? 4f : 2.5f;
                    var vm = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(new Vector3(-(_selected - i) * (_width / 5f), - scale * _playerSpriteSheets[i].SpriteHeight / 2, 0));
                    _3dSpriteShader.SetUniformMatrix4("view", false, ref vm);
                    _playerSpriteSheets[i].Draw(((int)(_time * 10)) % 10, 0);
                }

                _3dSpriteShader.Unbind();
            }

            {
                _flatColorShader.Bind();

                _flatColorShader.SetUniformMatrix4("proj", false, ref _guiProj);

                var viewModel = Matrix4.CreateTranslation(new Vector3(_text.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _text.Draw();

                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.Enter)
                MainWindow.SetGameState(new GameStatePlay(_playerSpriteSheets[_selected].TexturePath));
            if (key == Key.Left)
            {
                _selected--;
                if (_selected < 0)
                    _selected = 0;
            }
            if (key == Key.Right)
            {
                _selected++;
                if (_selected >= _playerSpriteSheets.Length)
                    _selected = _playerSpriteSheets.Length - 1;
            }
        }

        public void OnKeyPress(char key) { }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            _guiProj = Matrix4.CreateOrthographic(_width, _height, -1, 1);
            _text.Position = new Vector2(width / 2, 50);
        }

        public void Update(double deltaTime)
        {
            _time += (float)deltaTime;
        }
    }
}

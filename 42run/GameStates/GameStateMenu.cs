using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using _42run.OpenGL;
using OpenTK;

namespace _42run.GameStates
{
    public class GameStateMenu : IGameState
    {
        private Color4 _backColor;
        private Font _font;
        private Shader _flatColorShader;
        private Text _text;
        private Matrix4 _guiProj;

        private int _width;
        private int _height;

        public GameStateMenu()
        {
            _backColor = new Color4(0f, 0f, 0f, 1f);
            _font = FontManager.Get("glyphs");
            _flatColorShader = ShaderManager.Get("FlatColorShader");
            _text = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.MIDDLE, $"Press the Space button to start!");
        }

        public void Dispose()
        {
            _text?.Dispose();
            _text = null;
        }

        public void Draw(double deltaTime)
        {
            var textOffset = -new Vector2(_width / 2, _height / 2);
            GL.ClearColor(_backColor);
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            {
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                _flatColorShader.Bind();

                _flatColorShader.SetUniformMatrix4("proj", false, ref _guiProj);
                var viewModel = Matrix4.CreateTranslation(new Vector3(_text.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _text.Draw();
            }
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.Space)
                MainWindow.SetGameState(new GameStatePlay());
        }

        public void OnKeyPress(char key)
        {
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            _guiProj = Matrix4.CreateOrthographic(_width, _height, 0, 1);
            _text.Position = new Vector2(width / 2, 50);
        }

        public void Update(double deltaTime)
        {
        }
    }
}

using _42run.Gameplay;
using _42run.OpenGL;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _42run.GameStates
{
    public class GameStateGameOver : IGameState
    {
        private int _finalScore;

        private Shader _flatColorShader;

        private Text _scoreText;
        private Text _nameText;
        private Text _scoreboardText;
        private Text _scoreboardNameText;
        private Text _scoreboardScoreText;
        private Font _font;

        private Matrix4 _guiProj;

        private Color4 _backColor;

        private int _width;
        private int _height;
        private bool _lastCursorState;
        private double _time;

        private string _name;
        
        public GameStateGameOver(int finalScore)
        {
            _backColor = new Color4(0f, 0f, 0f, 1f);
            _finalScore = finalScore;
            _font = FontManager.Get("glyphs");
            _flatColorShader = ShaderManager.Get("FlatColorShader");
            _scoreText = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.MIDDLE, $"Your final score: {_finalScore}");
            _name = "";
            _nameText = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.LEFT, "");
            _scoreboardText = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.MIDDLE, "Scoreboard");
            _scoreboardNameText = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.RIGHT, "");
            _scoreboardScoreText = new Text(new Vector2(10, 10), _font, _flatColorShader, Text.Alignment.LEFT, "");
            DownloadScores();
        }

        public void Dispose()
        {
            _scoreText.Dispose();
            _scoreText = null;
            _nameText.Dispose();
            _nameText = null;
            _scoreboardText.Dispose();
            _scoreboardText = null;
            _scoreboardNameText.Dispose();
            _scoreboardNameText = null;
            _scoreboardScoreText.Dispose();
            _scoreboardScoreText = null;
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
                var viewModel = Matrix4.CreateTranslation(new Vector3(_scoreText.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreText.Draw();

                viewModel = Matrix4.CreateTranslation(new Vector3(_nameText.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _nameText.Draw();

                viewModel = Matrix4.CreateTranslation(new Vector3(_scoreboardNameText.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreboardNameText.Draw();

                viewModel = Matrix4.CreateTranslation(new Vector3(_scoreboardScoreText.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreboardScoreText.Draw();

                viewModel = Matrix4.CreateTranslation(new Vector3(_scoreboardText.Position + textOffset));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreboardText.Draw();
                
                _flatColorShader.Unbind();

                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.Enter)
            {
                SendScore();
                MainWindow.SetGameState(new GameStateMenu());
            }
            else if (key == Key.BackSpace)
            {
                if (_name.Length > 0)
                {
                    _name = _name.Substring(0, _name.Length - 1);
                    _nameText.Str = _name;
                    _nameText.Position = new Vector2(_width / 2 - _font.GetStringWidth(_nameText.Str) / 2, _height / 2 - _font.Texture.Height / 2 - 20);
                }
            }
        }

        protected void DownloadScores()
        {
            try
            {
                WebClient client = new WebClient();
                var scoresString = client.DownloadString("https://entrivax.fr/42run/scores");
                var scores = JsonConvert.DeserializeObject<List<ScoreObject>>(scoresString);
                var namesColumn = scores.Select(score => score.Name).Aggregate((a, b) => a + "\n" + b);
                var scoresColumn = scores.Select(score => score.Score.ToString()).Aggregate((a, b) => a + "\n" + b);
                _scoreboardNameText.Str = namesColumn;
                _scoreboardScoreText.Str = scoresColumn;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to retreive scores from the server:");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        protected void SendScore()
        {
            if (string.IsNullOrWhiteSpace(_name))
                return;
            try
            {
                WebClient client = new WebClient();
                var score = new ScoreObject { Name = _name, Score = _finalScore };
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.UploadString("https://entrivax.fr/42run/scores", "POST", JsonConvert.SerializeObject(score));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to send score to the server:");
                Console.WriteLine($"Message: {exception.Message}");
            }
        }

        public void OnKeyPress(char key)
        {
            if (_name.Length < 16)
            {
                _name += key;
                _nameText.Str = _name;
                _nameText.Position = new Vector2(_width / 2 - _font.GetStringWidth(_name) / 2, _height / 2 - _font.Texture.Height / 2 - 20);
            }
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            
            _guiProj = Matrix4.CreateOrthographic(_width, _height, 0, 1);
            _scoreText.Position = new Vector2(_width / 2, _height / 2 - _font.Texture.Height / 2 + 20);
            _nameText.Position = new Vector2(_width / 2 - _font.GetStringWidth(_name) / 2, _height / 2 - _font.Texture.Height / 2 - 20);
            _scoreboardText.Position = new Vector2((_width / 5) * 4, _height / 5 * 4 - _font.Texture.Height / 2 + 45);
            _scoreboardNameText.Position = new Vector2((_width / 5) * 4 - 10, _height / 5 * 4 - _font.Texture.Height / 2);
            _scoreboardScoreText.Position = new Vector2((_width / 5) * 4 + 10, _height / 5 * 4 - _font.Texture.Height / 2);
        }

        public void Update(double deltaTime)
        {
            _time += deltaTime;

            if ((int)(_time * 2) % 2 == 1)
            {
                if (!_lastCursorState)
                {
                    _nameText.Str = $"{_name}-";
                    _lastCursorState = true;
                }
            }
            else
            {
                if (_lastCursorState)
                {
                    _nameText.Str = _name;
                    _lastCursorState = false;
                }
            }
        }
    }
}

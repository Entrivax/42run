using _42run.Gameplay;
using _42run.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace _42run.GameStates
{
    public class GameStatePlay : IGameState
    {
        private Font _font;
        private Text _scoreText;

        private World _world;
        private Player _player;

        private Camera _camera;

        private Shader _flatColorShader;
        private Shader _3dSpriteShader;
        private Shader _baseShader;

        private SpriteSheet _playerSpriteSheet;
        private Texture _interLeftTex;
        private Texture _interRightTex;
        private Texture _interLeftRightTex;
        private Texture _wallTex;

        private Object3D _groundMesh;
        private Object3D _cubeMesh;
        private Object3D _interLeftMesh;
        private Object3D _interRightMesh;
        private Object3D _interLeftRightMesh;
        private Object3D _playerMesh;

        private Matrix4 _proj;
        private Matrix4 _guiProj;

        private bool _pause;
        private double _time;
        private Color4 _backColor;
        private int _width;
        private int _height;

        public GameStatePlay()
        {
            _backColor = new Color4(0f, 0f, 0f, 1f);

            _camera = new Camera(Vector3.Zero, Vector3.UnitZ, (float)(80f * (Math.PI / 180f)));

            // SHADERS RETRIEVING *********************************************************** //
            _flatColorShader = ShaderManager.Get("FlatColorShader");
            _baseShader = ShaderManager.Get("Shader");
            _3dSpriteShader = ShaderManager.Get("SpriteSheet");

            // FONT RETRIEVING ************************************************************** //
            _font = FontManager.Get("glyphs");
            // TEXT INITIALISATION ********************************************************** //
            _scoreText = new Text(new Vector2(13, 30), _font, _flatColorShader, "Test");

            // TEXTURES INITIALISATION ****************************************************** //
            /*_interLeftTex = new Texture("inter_l.png");
            _interRightTex = new Texture("inter_r.png");
            _interLeftRightTex = new Texture("inter_lr.png");
            _wallTex = new Texture("wall.png");*/
            _playerSpriteSheet = new SpriteSheet("running_link.png", 24, 32);

            // MESH INITIALISATION ********************************************************** //
            _groundMesh = new Object3D("wall.obj", false, false, true);
            _groundMesh.LoadInGl(_baseShader);

            GroundSimple.MeshToUse = _groundMesh;

            _cubeMesh = new Object3D("cube.obj", false, false, false);
            _cubeMesh.LoadInGl(_baseShader);

            AxisAlignedBB.SetMesh(_cubeMesh);

            _interLeftMesh = new Object3D("inter_l.obj", false, false, true);
            _interLeftMesh.LoadInGl(_baseShader);

            Intersection.Left_Mesh = _interLeftMesh;

            _interRightMesh = new Object3D("inter_r.obj", false, false, true);
            _interRightMesh.LoadInGl(_baseShader);

            Intersection.Right_Mesh = _interRightMesh;

            _interLeftRightMesh = new Object3D("inter_lr.obj", false, false, true);
            _interLeftRightMesh.LoadInGl(_baseShader);

            Intersection.LeftRight_Mesh = _interLeftRightMesh;

            var x = ((24 / 32f) * 1.7f) / 2;
            _playerMesh = new Object3D(new[] {new Mesh
            {
                Vertices = new List<Vertex>
                {
                    new Vertex(new Vector3(-x, 1.7f, 0f), new Vector2(0, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(1, 0)),

                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(1, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 0f, 0f), new Vector2(1, 1)),
                }
            }});
            _playerMesh.LoadInGl(_3dSpriteShader);

            // WORLD INITIALISATION ********************************************************* //
            _world = new World();
            // PLAYER INITIALISATION ******************************************************** //
            _player = new Player { World = _world, Position = new Vector3(0), Speed = 12.5f };

            // TERRAIN GENERATION *********************************************************** //
            var tilesToGenerate = 10;
            for (int i = 0; i < tilesToGenerate; i++)
            {
                _world.Grounds.Add(new Ground { BoundingBox = new AxisAlignedBB(new Vector3(-3f, -0.5f, -6f), new Vector3(3f, 0f, 0f)), Mesh = _groundMesh, Position = new Vector3(0, 0, -6f * i), Direction = Direction.NORTH });
            }
            var intersection = GroundFactory.NewIntersection(_player, _world, new Vector3(0, 0, -6f * tilesToGenerate), Direction.NORTH, (int)Intersection.IntersectionDirection.LEFT);
            _world.Grounds.Add(intersection);
            var rotation = DirectionHelper.GetRotationFromDirection(Direction.NORTH);
            var w1p1 = new Vector3(new Vector4(-3f, 0f, -7f, 1) * rotation);
            var w1p2 = new Vector3(new Vector4(3f, 5f, -6f, 1) * rotation);
            _world.Obstacles.Add(new Obstacle(null, new AxisAlignedBB(Vector3.ComponentMin(w1p1, w1p2), Vector3.ComponentMax(w1p1, w1p2)), intersection.Position));
            var w2p1 = new Vector3(new Vector4(3f, 0f, -6f, 1) * rotation);
            var w2p2 = new Vector3(new Vector4(4f, 5f, 0f, 1) * rotation);
            _world.Obstacles.Add(new Obstacle(null, new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position));
        }
        
        public void Update(double deltaTime)
        {
            if (KeyboardHelper.IsKeyPressed(Key.Space))
                _pause = !_pause;
            if (!_pause)
            {
                _player.Update(deltaTime);

                if (_player.Dead)
                    MainWindow.SetGameState(new GameStateGameOver((int)_player.Score));

                _scoreText.Str = $"Score: {(int)_player.Score}";
                _world.Update();

                var playerPosition = _player.PositionForCamera;

                _camera.Target = playerPosition + new Vector3(0, 2.5f, 0);
                //if (cam)
                    _camera.UpdateCameraPosition(playerPosition + (-DirectionHelper.GetVectorFromDirection(_player.CurrentDirection) * 4f) + new Vector3(0, 3, 0), (float)deltaTime, 5f);
                // DEBUG CAM
                /*else
                    _camera.Position = new Vector3(playerPosition.X, 10, playerPosition.Z) + (-DirectionHelper.GetVectorFromDirection(_player.CurrentDirection) * 4f);*/
            }
        }

        public void Draw(double deltaTime)
        {
            _time += deltaTime;
            GL.ClearColor(_backColor);
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = _camera.ComputeViewMatrix();
            var model = Matrix4.CreateTranslation(_player.GetPosition());

            var viewModel = model * view;

            {
                _baseShader.Bind();

                _baseShader.SetUniformMatrix4("proj", false, ref _proj);


                model = _player.BoundingBox.CreateModelMatrix() * model;
                viewModel = model * view;
                _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                //_player.BoundingBox.Draw();

                var color = new Vector3(1, 1, 1);

                foreach (var ground in _world.Grounds.ToArray())
                {

                    model = ground.BoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(ground.Position);
                    viewModel = model * view;
                    color = new Vector3(0.7f, 0.7f, 0.7f);
                    _baseShader.SetUniform3("col", ref color);
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    if (ground.GetType() == typeof(Intersection))
                    {
                        //ground.BoundingBox.Draw();
                        var inter = (Intersection)ground;

                        model = DirectionHelper.GetRotationFromDirection(inter.Direction) * Matrix4.CreateTranslation(inter.Position);
                        viewModel = model * view;
                        color = new Vector3(0.8f, 0.8f, 0.8f);
                        _baseShader.SetUniform3("col", ref color);
                        _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                        if (inter.Directions == 1)
                            inter.Mesh.Draw();
                        else if (inter.Directions == 2)
                            inter.Mesh.Draw();
                        else
                            inter.Mesh.Draw();

                        model = inter.ActivableBoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(ground.Position);
                        viewModel = model * view;
                        color = new Vector3(0, 1, 0);
                        _baseShader.SetUniform3("col", ref color);
                        _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                        //inter.ActivableBoundingBox.Draw();
                    }
                    else
                    {
                        //ground.BoundingBox.Draw();
                        model = DirectionHelper.GetRotationFromDirection(ground.Direction) * Matrix4.CreateTranslation(ground.Position);
                        viewModel = model * view;
                        color = new Vector3(0.8f, 0.8f, 0.8f);
                        _baseShader.SetUniform3("col", ref color);
                        _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                        ground.Mesh.Draw();
                    }
                }

                foreach (var obstacle in _world.Obstacles.ToArray())
                {
                    model = Matrix4.CreateTranslation(obstacle.Position);
                    viewModel = model * view;
                    color = new Vector3(0.7f, 0.2f, 1f);
                    _baseShader.SetUniform3("col", ref color);
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.Mesh?.Draw();
                    model = obstacle.BoundingBox.CreateModelMatrix() * model;
                    viewModel = model * view;
                    color = new Vector3(0.1f, 0.2f, 1f);
                    _baseShader.SetUniform3("col", ref color);
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.BoundingBox.Draw();
                }

                color = new Vector3(1f, 0.6f, 0.2f);
                _baseShader.SetUniform3("col", ref color);
                foreach (var trigger in _world.Triggers.ToArray())
                {
                    model = trigger.BoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(trigger.Position);
                    viewModel = model * view;
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    //trigger.BoundingBox.Draw();
                }

                _baseShader.Unbind();
            }

            {
                _3dSpriteShader.Bind();

                GL.BindTexture(TextureTarget.Texture2DArray, _playerSpriteSheet.Id);

                _3dSpriteShader.SetUniformMatrix4("proj", false, ref _proj);
                model = DirectionHelper.GetRotationFromDirection(_player.CurrentDirection) * Matrix4.CreateTranslation(_player.GetPosition());
                viewModel = model * view;
                _3dSpriteShader.SetUniformMatrix4("view", false, ref viewModel);
                _3dSpriteShader.SetUniform1("texNum", ((int)(_time * 10)) % 10);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                _playerMesh.Draw();

                GL.Disable(EnableCap.Blend);

                _3dSpriteShader.Unbind();
            }
            
            {
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                _flatColorShader.Bind();

                //_guiFramebuffer.Bind();
                _flatColorShader.SetUniformMatrix4("proj", false, ref _guiProj);
                viewModel = Matrix4.CreateTranslation(new Vector3(_scoreText.Position + -new Vector2(_width / 2, _height / 2)));
                _flatColorShader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreText.Draw();

                //_guiFramebuffer.Unbind();

                _flatColorShader.Unbind();
                
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;

            _proj = _camera.ComputeProjectionMatrix(_width / (float)_height);
            _guiProj = Matrix4.CreateOrthographic(_width, _height, 0, 1);
        }

        public void Dispose()
        {
            _scoreText?.Dispose();
            _scoreText = null;
            _groundMesh?.Dispose();
            _groundMesh = null;
            _cubeMesh?.Dispose();
            _cubeMesh = null;
            _interLeftMesh?.Dispose();
            _interLeftMesh = null;
            _interRightMesh?.Dispose();
            _interRightMesh = null;
            _interLeftRightMesh?.Dispose();
            _interLeftRightMesh = null;
            _playerMesh?.Dispose();
            _playerMesh = null;
            _playerSpriteSheet?.Dispose();
            _playerSpriteSheet = null;
            _interLeftTex?.Dispose();
            _interLeftTex = null;
            _interRightTex?.Dispose();
            _interRightTex = null;
            _interLeftRightTex?.Dispose();
            _interLeftRightTex = null;
            _wallTex?.Dispose();
            _wallTex = null;
        }

        public void OnKeyPress(char key) { }

        public void OnKeyDown(Key key) { }
    }
}

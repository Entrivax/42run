using _42run.Gameplay;
using _42run.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace _42run
{
    public class MainWindow : GameWindow
    {
        private Shader _shader;
        private Camera _camera;
        private Matrix4 _proj;
        private Matrix4 _guiProj;
        private double _time;
        private World _world;
        private Player _player;
        private Mesh _groundMesh;
        private Mesh _cubeMesh;
        private Shader _3dSpriteShader;
        private SpriteSheet _playerSpriteSheet;
        private Mesh _playerMesh;
        private Mesh _interLeftMesh;
        private Mesh _interRightMesh;
        private Mesh _interLeftRightMesh;
        private Texture _interLeftTex;
        private Texture _interRightTex;
        private Texture _interLeftRightTex;
        private Texture _wallTex;
        private Font _font;
        private Shader _flatColorShader;
        private Text _scoreText;

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            //VSync = VSyncMode.Off;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _proj = _camera.ComputeProjectionMatrix(Width / (float)Height);
            _guiProj = Matrix4.CreateOrthographic(Width, Height, 0, 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            _camera = new Camera(Vector3.Zero, Vector3.UnitZ, (float)(80f * (Math.PI / 180f)));
            Closed += OnClosed;
            CursorVisible = true;

            _shader = new Shader("Shaders/Shader.vs", "Shaders/Shader.fs");
            _flatColorShader = new Shader("Shaders/FlatColorShader.vs", "Shaders/FlatColorShader.fs");
            _3dSpriteShader = new Shader("Shaders/SpriteSheet.vs", "Shaders/SpriteSheet.fs");

            _font = new Font("glyphs.png", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789=*-+[]{}()|\\,./<>?;:'\"");
            _scoreText = new Text(new Vector2(10, 10), _font, _flatColorShader, "Test");

            _interLeftTex = new Texture("inter_l.png");
            _interRightTex = new Texture("inter_r.png");
            _interLeftRightTex = new Texture("inter_lr.png");
            _wallTex = new Texture("wall.png");

            _groundMesh = new Mesh();
            _groundMesh.LoadFile("wall.obj", false, false, true);
            _groundMesh.LoadInGl(_shader);
            
            GroundSimple.MeshToUse = _groundMesh;

            _cubeMesh = new Mesh();
            _cubeMesh.LoadFile("cube.obj", false, false, false);
            _cubeMesh.LoadInGl(_shader);

            _interLeftMesh = new Mesh();
            _interLeftMesh.LoadFile("inter_l.obj", false, false, true);
            _interLeftMesh.LoadInGl(_shader);

            Intersection.Left_Mesh = _interLeftMesh;

            _interRightMesh = new Mesh();
            _interRightMesh.LoadFile("inter_r.obj", false, false, true);
            _interRightMesh.LoadInGl(_shader);

            Intersection.Right_Mesh = _interRightMesh;

            _interLeftRightMesh = new Mesh();
            _interLeftRightMesh.LoadFile("inter_lr.obj", false, false, true);
            _interLeftRightMesh.LoadInGl(_shader);

            Intersection.LeftRight_Mesh = _interLeftRightMesh;

            AxisAlignedBB.SetMesh(_cubeMesh);

            _playerSpriteSheet = new SpriteSheet("running_link.png", 24, 32);
            var x = ((24 / 32f) * 1.7f) / 2;
            _playerMesh = new Mesh
            {
                Vertices = new Vertex[]
                {
                    new Vertex(new Vector3(-x, 1.7f, 0f), new Vector2(0, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(1, 0)),

                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(1, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 0f, 0f), new Vector2(1, 1)),
                }
            };
            _playerMesh.LoadInGl(_3dSpriteShader);

            _world = new World();
            _player = new Player { World = _world, Position = new Vector3(0), Speed = 12.5f };
            for(int i = 0; i < 25; i++)
            {
                _world.Grounds.Add(new Ground { BoundingBox = new AxisAlignedBB(new Vector3(-3f, -0.5f, -6f), new Vector3(3f, 0f, 0f)), Mesh = _groundMesh, Position = new Vector3(0, 0, -6f * i), Direction = Direction.NORTH });
            }
            _world.Grounds.Add(GroundFactory.NewIntersection(_player, _world, new Vector3(0, 0, -6f * 25), Direction.NORTH, (int)Intersection.IntersectionDirection.LEFT));
            //_world.Grounds.Add(new Intersection(_player, _world, new Vector3(0, 0, -6f * 25 - 0.3f), Direction.NORTH) { BoundingBox = new AxisAlignedBB(new Vector3(-3f, -0.5f, -2f), new Vector3(3f, 0f, 2f)), ActivableBoundingBox = new AxisAlignedBB(new Vector3(-3f, 0f, -1f), new Vector3(3f, 3f, 1f)), Mesh = _groundMesh, Directions = (int)Intersection.IntersectionDirection.LEFT });
            _world.Obstacles.Add(new Obstacle(null, new AxisAlignedBB(new Vector3(-1, 0, -1), new Vector3(1, 3, 1)), new Vector3(new Random().Next(3), 0, -20)));
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_shader != null)
                _shader.Dispose();
            _shader = null;
            base.OnUnload(e);
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void HandleKeyboard()
        {
            KeyboardHelper.Update();

            if (KeyboardHelper.IsKeyPressed(Key.Escape))
            {
                Exit();
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();

            if (KeyboardHelper.IsKeyPressed(Key.A))
                cam = !cam;
            if (KeyboardHelper.IsKeyPressed(Key.Space))
                pause = !pause;
            if (!pause)
            {
                _player.Update(e.Time);
                _scoreText.Str = $"Score: {(int)_player.Score}";
                _world.Update();
            }
        }

        bool pause = false;
        bool cam = true;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += e.Time;
            Title = $"42run ; FPS: {Math.Round(1 / e.Time)} ; v0.01";
            var backColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            GL.ClearColor(backColor);
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            var playerPosition = _player.PositionForCamera;

            _camera.Target = playerPosition + new Vector3(0, 2.5f, 0);
            if (cam)
                _camera.UpdateCameraPosition(playerPosition + (-DirectionHelper.GetVectorFromDirection(_player.CurrentDirection) * 4f) + new Vector3(0, 3, 0), (float)e.Time, 5f);
            // DEBUG CAM
            else
                _camera.Position = new Vector3(playerPosition.X, 10, playerPosition.Z) + (-DirectionHelper.GetVectorFromDirection(_player.CurrentDirection) * 4f);
            var view = _camera.ComputeViewMatrix();
            var model = Matrix4.CreateTranslation(_player.GetPosition());

            var viewModel = model * view;

            {
                _shader.Bind();

                _shader.SetUniformMatrix4("proj", false, ref _proj);


                model = _player.BoundingBox.CreateModelMatrix() * model;
                viewModel = model * view;
                _shader.SetUniformMatrix4("view", false, ref viewModel);
                //_player.BoundingBox.Draw();

                var color = new Vector3(1, 1, 1);

                foreach (var ground in _world.Grounds.ToArray())
                {
                    
                    model = ground.BoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(ground.Position);
                    viewModel = model * view;
                    color = new Vector3(0.7f, 0.7f, 0.7f);
                    _shader.SetUniform3("col", ref color);
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    if (ground.GetType() == typeof(Intersection))
                    {
                        //ground.BoundingBox.Draw();
                        var inter = (Intersection)ground;

                        model = DirectionHelper.GetRotationFromDirection(inter.Direction) * Matrix4.CreateTranslation(inter.Position);
                        viewModel = model * view;
                        color = new Vector3(0.8f, 0.8f, 0.8f);
                        _shader.SetUniform3("col", ref color);
                        _shader.SetUniformMatrix4("view", false, ref viewModel);
                        if (inter.Directions == 1)
                            inter.Mesh.Draw(_interLeftTex);
                        else if (inter.Directions == 2)
                            inter.Mesh.Draw(_interRightTex);
                        else
                            inter.Mesh.Draw(_interLeftRightTex);

                        model = inter.ActivableBoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(ground.Position);
                        viewModel = model * view;
                        color = new Vector3(0, 1, 0);
                        _shader.SetUniform3("col", ref color);
                        _shader.SetUniformMatrix4("view", false, ref viewModel);
                        //inter.ActivableBoundingBox.Draw();
                    }
                    else
                    {
                        //ground.BoundingBox.Draw();
                        model = DirectionHelper.GetRotationFromDirection(ground.Direction) * Matrix4.CreateTranslation(ground.Position);
                        viewModel = model * view;
                        color = new Vector3(0.8f, 0.8f, 0.8f);
                        _shader.SetUniform3("col", ref color);
                        _shader.SetUniformMatrix4("view", false, ref viewModel);
                        ground.Mesh.Draw(_wallTex);
                    }
                }

                foreach (var obstacle in _world.Obstacles.ToArray())
                {
                    model = Matrix4.CreateTranslation(obstacle.Position);
                    viewModel = model * view;
                    color = new Vector3(0.7f, 0.2f, 1f);
                    _shader.SetUniform3("col", ref color);
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.Mesh?.Draw();
                    model = obstacle.BoundingBox.CreateModelMatrix() * model;
                    viewModel = model * view;
                    color = new Vector3(0.1f, 0.2f, 1f);
                    _shader.SetUniform3("col", ref color);
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.BoundingBox.Draw();
                }

                color = new Vector3(1f, 0.6f, 0.2f);
                _shader.SetUniform3("col", ref color);
                foreach (var trigger in _world.Triggers.ToArray())
                {
                    model = trigger.BoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(trigger.Position);
                    viewModel = model * view;
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    //trigger.BoundingBox.Draw();
                }

                _shader.Unbind();
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
                _shader.SetUniformMatrix4("proj", false, ref _guiProj);
                viewModel = Matrix4.CreateTranslation(new Vector3(_scoreText.Position + -new Vector2(Width / 2, Height / 2)));
                _shader.SetUniformMatrix4("view", false, ref viewModel);

                _scoreText.Draw();

                //_guiFramebuffer.Unbind();

                _flatColorShader.Unbind();




                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
            }

            SwapBuffers();
        }
    }
}

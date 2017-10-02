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
        private double _time;
        private Mesh _testMesh;
        private World _world;
        private Player _player;
        private Mesh _groundMesh;
        private Mesh _cubeMesh;
        private Shader _3dSpriteShader;
        private SpriteSheet _playerSpriteSheet;
        private Mesh _playerMesh;

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            //VSync = VSyncMode.Off;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _proj = _camera.ComputeProjectionMatrix(Width / (float)Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            _camera = new Camera(Vector3.Zero, Vector3.UnitZ, (float)(80f * (Math.PI / 180f)));
            Closed += OnClosed;
            CursorVisible = true;
            _shader = new Shader("Shaders/Shader.vs", "Shaders/Shader.fs");
            _3dSpriteShader = new Shader("Shaders/SpriteSheet.vs", "Shaders/SpriteSheet.fs");
            _testMesh = new Mesh();
            _testMesh.LoadFile("player.obj");
            _testMesh.LoadInGl(_shader);

            _groundMesh = new Mesh();
            _groundMesh.LoadFile("walls.obj");
            _groundMesh.LoadInGl(_shader);

            _cubeMesh = new Mesh();
            _cubeMesh.LoadFile("cube.obj");
            _cubeMesh.LoadInGl(_shader);

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
            _player = new Player { World = _world, Position = new Vector3(0), Speed = 12.5f, Direction = new Vector3(0, 0, -1) };
            for(int i = 0; i < 25; i++)
            {
                _world.Grounds.Add(new Ground { BoundingBox = new AxisAlignedBB(new Vector3(-3f, -0.5f, -2f), new Vector3(3f, 0f, 2f)), Mesh = _groundMesh, Position = new Vector3(0, 0, -4f * i) });
            }
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

            _player.Update(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += e.Time;
            Title = $"42run ; FPS: {1 / e.Time} ; v0.01";
            var backColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            GL.ClearColor(backColor);
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var playerPosition = _player.PositionForCamera;

            _camera.Target = playerPosition + new Vector3(0, 2.5f, 0);
            var phi = _time / Math.PI;
            _camera.Position = playerPosition + (-_player.Direction * 4f) + new Vector3(0, 3, 0);
            var view = _camera.ComputeViewMatrix();
            var model = Matrix4.CreateTranslation(_player.GetPosition());

            var viewModel = model * view;

            {
                _shader.Bind();

                _shader.SetUniformMatrix4("proj", false, ref _proj);


                model = Player.BoundingBox.CreateModelMatrix() * model;
                viewModel = model * view;
                _shader.SetUniformMatrix4("view", false, ref viewModel);
                Player.BoundingBox.Draw();

                foreach (var ground in _world.Grounds)
                {
                    model = Matrix4.CreateTranslation(ground.Position);
                    viewModel = model * view;
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    ground.Mesh.Draw();
                    model = ground.BoundingBox.CreateModelMatrix() * model;
                    viewModel = model * view;
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    ground.BoundingBox.Draw();
                }

                foreach (var obstacle in _world.Obstacles)
                {
                    model = Matrix4.CreateTranslation(obstacle.Position);
                    viewModel = model * view;
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.Mesh?.Draw();
                    model = obstacle.BoundingBox.CreateModelMatrix() * model;
                    viewModel = model * view;
                    _shader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.BoundingBox.Draw();
                }

                _shader.Unbind();
            }

            {
                _3dSpriteShader.Bind();

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, _playerSpriteSheet.Id);

                _3dSpriteShader.SetUniformMatrix4("proj", false, ref _proj);
                model = Matrix4.CreateTranslation(_player.GetPosition());
                viewModel = model * view;
                _3dSpriteShader.SetUniformMatrix4("view", false, ref viewModel);
                _3dSpriteShader.SetUniform1("texNum", ((int)(_time * 7)) % 10);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                _playerMesh.Draw();

                GL.Disable(EnableCap.Blend);

                _3dSpriteShader.Unbind();
            }

            SwapBuffers();
        }
    }
}

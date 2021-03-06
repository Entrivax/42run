﻿using _42run.Gameplay;
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

        private Object3D _groundMesh;
        private Object3D _groundStairsMesh;
        private Object3D _groundClusterMesh;
        private Object3D _cubeMesh;
        private Object3D _interLeftMesh;
        private Object3D _interRightMesh;
        private Object3D _interLeftRightMesh;
        private Object3D _trashMesh;
        private Object3D _coinMesh;
        private Object3D _playerMesh;

        private Matrix4 _proj;
        private Matrix4 _guiProj;
        
        private double _time;
        private Color4 _backColor;
        private int _width;
        private int _height;
        private float _coinsRotation;

        public GameStatePlay(string playerSkin)
        {
            _backColor = new Color4(0.6f, 0.8f, 0.85f, 1f);
            
            // SHADERS RETRIEVING *********************************************************** //
            _flatColorShader = ShaderManager.Get("FlatColorShader");
            _baseShader = ShaderManager.Get("Shader");
            _3dSpriteShader = ShaderManager.Get("SpriteSheet");

            // FONT RETRIEVING ************************************************************** //
            _font = FontManager.Get("glyphs");
            // TEXT INITIALISATION ********************************************************** //
            _scoreText = new Text(new Vector2(13, 30), _font, _flatColorShader, "Test");
            
            var x = ((24 / 32f) * 1.7f) / 2;
            var u = 24.0f / 240;
            _playerMesh = new Object3D(new[] {new Mesh
            {
                Vertices = new List<Vertex>
                {
                    new Vertex(new Vector3(-x, 1.7f, 0f), new Vector2(0, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(u, 0)),

                    new Vertex(new Vector3(x, 1.7f, 0f), new Vector2(u, 0)),
                    new Vertex(new Vector3(-x, 0f, 0f), new Vector2(0, 1)),
                    new Vertex(new Vector3(x, 0f, 0f), new Vector2(u, 1)),
                }
            }});
            _playerMesh.LoadInGl(_3dSpriteShader);
            // TEXTURES INITIALISATION ****************************************************** //
            _playerSpriteSheet = new SpriteSheet(_playerMesh, playerSkin, 24, 32, TextureMinFilter.Nearest, TextureMagFilter.Nearest);

            // MESH INITIALISATION ********************************************************** //
            _groundMesh = new Object3D("wall.obj", false, false, true);
            _groundMesh.LoadInGl(_baseShader);

            GroundSimple.MeshToUse = _groundMesh;

            _groundStairsMesh = new Object3D("stairs.obj", false, false, true);
            _groundStairsMesh.LoadInGl(_baseShader);

            GroundStairs.MeshToUse = _groundStairsMesh;

            _groundClusterMesh = new Object3D("cluster.obj", false, false, true);
            _groundClusterMesh.LoadInGl(_baseShader);

            GroundCluster.MeshToUse = _groundClusterMesh;

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

            _trashMesh = new Object3D("trash.obj", false, false, true);
            _trashMesh.LoadInGl(_baseShader);

            ObstacleTrash.MeshToUse = _trashMesh;

            _coinMesh = new Object3D("coin.obj", false, false, true);
            _coinMesh.LoadInGl(_baseShader);

            Coin.MeshToUse = _coinMesh;


            // WORLD INITIALISATION ********************************************************* //
            _world = new World();
            // PLAYER INITIALISATION ******************************************************** //
            _player = new Player { World = _world, Position = new Vector3(0, 0, -3f), Speed = 12.5f };
            _camera = new Camera(new Vector3(0, 2f, 2f), _player.PositionForCamera + new Vector3(0, 2.5f, 0), (float)(80f * (Math.PI / 180f)));

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
            _world.Obstacles.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w1p1, w1p2), Vector3.ComponentMax(w1p1, w1p2)), intersection.Position, Direction.NORTH));
            var w2p1 = new Vector3(new Vector4(3f, 0f, -6f, 1) * rotation);
            var w2p2 = new Vector3(new Vector4(4f, 5f, 0f, 1) * rotation);
            _world.Obstacles.Add(new Obstacle(new AxisAlignedBB(Vector3.ComponentMin(w2p1, w2p2), Vector3.ComponentMax(w2p1, w2p2)), intersection.Position, Direction.NORTH));

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        public void Update(double deltaTime)
        {
            _player.Update(deltaTime);

            if (_player.Dead)
                MainWindow.SetGameState(new GameStateGameOver((int)_player.Score));

            _scoreText.Str = $"Score: {(int)_player.Score}";
            _world.Update();

            var playerPosition = _player.PositionForCamera;

            _camera.Target = playerPosition + new Vector3(0, 2.5f, 0);
            _camera.UpdateCameraPosition(playerPosition + (-DirectionHelper.GetVectorFromDirection(_player.CurrentDirection) * 4f) + new Vector3(0, 3, 0), (float)deltaTime, 5f);
            _coinsRotation += 0.1f;
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

                var color = new Vector3(1, 1, 1);

                foreach (var ground in _world.Grounds.ToArray())
                {
                    if (ground.GetType() == typeof(Intersection))
                    {
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
                    }
                    else
                    {
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
                    model = DirectionHelper.GetRotationFromDirection(obstacle.Direction) * Matrix4.CreateTranslation(obstacle.Position);
                    viewModel = model * view;
                    color = new Vector3(0.7f, 0.2f, 1f);
                    _baseShader.SetUniform3("col", ref color);
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    obstacle.Mesh?.Draw();
                }

                color = new Vector3(1f, 0.6f, 0.2f);
                _baseShader.SetUniform3("col", ref color);
                foreach (var trigger in _world.Triggers.ToArray())
                {
                    model = trigger.BoundingBox.CreateModelMatrix() * Matrix4.CreateTranslation(trigger.Position);
                    viewModel = model * view;
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                }

                var coinRotationMatrix = Matrix4.CreateRotationY(_coinsRotation);
                foreach (var coin in _world.Coins.ToArray())
                {
                    model = coinRotationMatrix * Matrix4.CreateTranslation(coin.Position);
                    viewModel = model * view;
                    color = new Vector3(1f, 1f, 1f);
                    _baseShader.SetUniform3("col", ref color);
                    _baseShader.SetUniformMatrix4("view", false, ref viewModel);
                    coin.Mesh.Draw();
                }

                _baseShader.Unbind();
            }

            {
                _3dSpriteShader.Bind();

                _3dSpriteShader.SetUniformMatrix4("proj", false, ref _proj);
                model = DirectionHelper.GetRotationFromDirection(_player.CurrentDirection) * Matrix4.CreateTranslation(_player.GetPosition());
                viewModel = model * view;
                _3dSpriteShader.SetUniformMatrix4("view", false, ref viewModel);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                _playerSpriteSheet.Draw(_player.AnimationFrame, 0);

                GL.Disable(EnableCap.Blend);

                _3dSpriteShader.Unbind();
            }
            
            {
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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
            _scoreText.Dispose();
            _scoreText = null;
            _groundMesh.Dispose();
            _groundMesh = null;
            _groundStairsMesh.Dispose();
            _groundStairsMesh = null;
            _groundClusterMesh.Dispose();
            _groundClusterMesh = null;
            _cubeMesh.Dispose();
            _cubeMesh = null;
            _interLeftMesh.Dispose();
            _interLeftMesh = null;
            _interRightMesh.Dispose();
            _interRightMesh = null;
            _interLeftRightMesh.Dispose();
            _interLeftRightMesh = null;
            _trashMesh.Dispose();
            _trashMesh = null;
            _playerMesh.Dispose();
            _playerMesh = null;
            _playerSpriteSheet.Dispose();
            _playerSpriteSheet = null;
        }

        public void OnKeyPress(char key) { }

        public void OnKeyDown(Key key) { }
    }
}

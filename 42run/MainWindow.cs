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

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.Off;
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
            _testMesh = new Mesh();
            _testMesh.LoadFile("MOON.OBJ");
            _testMesh.LoadInGl(_shader);
            Console.WriteLine($"Model loaded with {_testMesh.Vertices.Length} vertices");
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        public override void Exit()
        {
            if (_shader != null)
                _shader.Dispose();
            _shader = null;
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
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += e.Time;
            Title = $"42run ; FPS: {1 / e.Time} ; v0.01";
            var backColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            GL.ClearColor(backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _camera.Target = new Vector3(0);
            var phi = _time / Math.PI;
            _camera.Position = new Vector3((float)Math.Cos(phi) * 10, 0, (float)Math.Sin(phi) * 10);
            var view = _camera.ComputeViewMatrix();

            {
                _shader.Bind();

                _shader.SetUniformMatrix4("proj", false, ref _proj);
                _shader.SetUniformMatrix4("view", false, ref view);

                _testMesh.Draw();

                _shader.Unbind();
            }

            SwapBuffers();
        }
    }
}

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
        private double _time;

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            _camera = new Camera(Vector3.Zero, Vector3.UnitZ, (float)(80f * (Math.PI / 180f)));
            Closed += OnClosed;
            CursorVisible = true;
            _shader = new Shader("Shaders/Shader.vs", "Shaders/Shader.fs");
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
            var backColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            GL.ClearColor(backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            SwapBuffers();
        }
    }
}

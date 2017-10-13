using _42run.Gameplay;
using _42run.GameStates;
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
        IGameState _gameState;
        public static MainWindow Instance;

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            Instance = this;
            //VSync = VSyncMode.Off;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _gameState?.Resize(Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            Closed += OnClosed;
            CursorVisible = true;
            KeyPress += OnKeyPress;
            KeyDown += OnKeyDown;
            SetInstanceGameState(new GameStatePlay());
            base.OnLoad(e);
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _gameState?.OnKeyDown(e.Key);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            _gameState?.OnKeyPress(e.KeyChar);
        }

        protected override void OnUnload(EventArgs e)
        {
            _gameState?.Dispose();
            _gameState = null;
            base.OnUnload(e);
        }

        protected void SetInstanceGameState(IGameState gameState)
        {
            _gameState = gameState;
            _gameState.Resize(Width, Height);
        }

        public static void SetGameState(IGameState gameState)
        {
            Instance.SetInstanceGameState(gameState);
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

            _gameState?.Update(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _gameState?.Draw(e.Time);

            SwapBuffers();
        }
    }
}

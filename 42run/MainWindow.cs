﻿using _42run.GameStates;
using _42run.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace _42run
{
    public class MainWindow : GameWindow
    {
        IGameState _gameState;
        public static MainWindow Instance;
        private bool _skipNextUpdate = false;

        public MainWindow() : base(1280, 720, GraphicsMode.Default, "42run", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            Instance = this;
            //VSync = VSyncMode.Off;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _skipNextUpdate = true;
            _gameState?.Resize(Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Closed += OnClosed;
                CursorVisible = true;
                KeyPress += OnKeyPress;
                KeyDown += OnKeyDown;
                SetInstanceGameState(new GameStateMenu());
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Une exception de type {exception.GetType()} est survenue, message : {exception.Message}");
                Console.WriteLine("Sortie...");
                Environment.Exit(1);
            }
            base.OnLoad(e);
        }

        protected override void OnMove(EventArgs e)
        {
            _skipNextUpdate = true;
            base.OnMove(e);
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
            FontManager.Clear();
            ShaderManager.Clear();
            TextureManager.Clear();
            base.OnUnload(e);
        }

        protected void SetInstanceGameState(IGameState gameState)
        {
            _gameState = gameState;
            _gameState.Resize(Width, Height);
            _skipNextUpdate = true;
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
            if (_skipNextUpdate)
            {
                _skipNextUpdate = false;
                return;
            }

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

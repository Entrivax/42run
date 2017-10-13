using OpenTK.Input;
using System;

namespace _42run.GameStates
{
    public interface IGameState : IDisposable
    {
        void Update(double deltaTime);
        void Draw(double deltaTime);

        void OnKeyPress(char key);
        void OnKeyDown(Key key);

        void Resize(int width, int height);
    }
}

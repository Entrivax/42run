using System;

namespace _42run.GameStates
{
    public interface IGameState : IDisposable
    {
        void Update(double deltaTime);
        void Draw(double deltaTime);

        void Resize(int width, int height);
    }
}

using System;
using FusionCore.Test.Models;
using UniRx;

namespace FusionCore.Test
{
    public class FightController : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public FightController(GameModel gameModel)
        {
            gameModel.CurrentGameState.Subscribe(OnChangeGameState).AddTo(_disposables);
            OnChangeGameState(GameState.MainMenu);
        }
        
        private void OnChangeGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    break;
                
                case GameState.Fight:
                    break;
                
                case GameState.EndFight:
                    break;
            }
        }
        
        
        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}

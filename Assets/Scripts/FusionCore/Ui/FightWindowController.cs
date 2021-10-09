using System;
using FusionCore.Test;
using FusionCore.Test.Models;
using UniRx;

namespace FusionCore.Ui
{
    public class FightWindowController : IDisposable
    {
        private FightWindowView _fightWindowView;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public FightWindowController(FightWindowView fightWindowView, GameModel gameModel)
        {
            _fightWindowView = fightWindowView;
            
            gameModel.CurrentGameState.Subscribe(OnChangeGameState).AddTo(_disposables);
            OnChangeGameState(GameState.MainMenu);
        }

        private void OnChangeGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    _fightWindowView.gameObject.SetActive(false);
                    break;
				
                case GameState.Fight:
                    _fightWindowView.gameObject.SetActive(true);
                    break;
                
				
                case GameState.EndFight:
                    _fightWindowView.gameObject.SetActive(false);
                    break;
            }
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}

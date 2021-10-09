using System;
using FusionCore.Test;
using FusionCore.Test.Models;
using UniRx;

namespace FusionCore.Ui
{
    public class MainMenuController : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        private MainMenuView _mainMenuView;
        private GameModel _gameModel;
        
        public MainMenuController(MainMenuView mainMenuView, GameModel gameModel)
        {
            _mainMenuView = mainMenuView;
            _gameModel = gameModel;
            
            SubscribeButtons();
            
            gameModel.CurrentGameState.Subscribe(RefreshWindow).AddTo(_disposables);
            RefreshWindow(GameState.MainMenu);
        }

        private void SubscribeButtons()
        {
            _mainMenuView.ContinueButton.OnClickAsObservable().Subscribe(_ => ContinueGame()).AddTo(_disposables);
            _mainMenuView.StartOverButton.OnClickAsObservable().Subscribe(_ => StartOverGame()).AddTo(_disposables);
        }

        private void StartOverGame()
        {
            _gameModel.CurrentGameState.Value = GameState.MainMenu;
        }

        private void ContinueGame()
        {
            _gameModel.CurrentGameState.Value = GameState.Fight;
        }

        public void RefreshWindow(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    _mainMenuView.gameObject.SetActive(true);
                    ChangeVisibleButton(true, false);
                    break;
                
                case GameState.Fight:
                    _mainMenuView.gameObject.SetActive(false);
                    break;
                
                case GameState.EndFight:
                    _mainMenuView.gameObject.SetActive(true);
                    ChangeVisibleButton(false, true);
                    break;
            }
        }

        private void ChangeVisibleButton(bool isContinueButton, bool isStartOverButton)
        {
            _mainMenuView.ContinueButton.gameObject.SetActive(isContinueButton);
            _mainMenuView.StartOverButton.gameObject.SetActive(isStartOverButton);
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
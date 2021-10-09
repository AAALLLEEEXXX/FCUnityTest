using FusionCore.Test;

namespace FusionCore.Ui
{
    public class MainMenuController
    {
        private MainMenuView _mainMenuView;
        
        public MainMenuController(MainMenuView mainMenuView)
        {
            _mainMenuView = mainMenuView;

            SubscribeButtons();
        }

        private void SubscribeButtons()
        {
            
        }

        private void RefreshWindow(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    _mainMenuView.gameObject.SetActive(true);
                    ChangeVisibleButton(true, false);
                    break;
                
                case GameState.Game:
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
    }
}

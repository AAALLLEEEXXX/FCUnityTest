using UnityEngine;
using UnityEngine.UI;

namespace FusionCore.Ui
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] 
        private Button _continueButton;

        [SerializeField] 
        private Button _startOverButton;

        public Button ContinueButton => _continueButton;

        public Button StartOverButton => _startOverButton;
    }
}
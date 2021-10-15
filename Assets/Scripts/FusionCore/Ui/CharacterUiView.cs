using UnityEngine;
using UnityEngine.UI;

namespace FusionCore.Ui
{
    public class CharacterUiView : MonoBehaviour
    {
        [SerializeField] 
        private Image _imageArmor;

        [SerializeField] 
        private Image _imageHealth;

        public float FillArmor
        {
            get => _imageArmor.fillAmount;
            set
            {
                if (Mathf.Approximately(_imageArmor.fillAmount, value)) 
                    return;
                
                if (value <= 0)
                    _imageArmor.fillAmount = 0;
                else
                    _imageArmor.fillAmount = value;
            }
        } 

        public float FillHealth
        {
            get => _imageHealth.fillAmount;
            set
            {
                if (value <= 0)
                    gameObject.SetActive(false);
                
                if (!Mathf.Approximately(_imageHealth.fillAmount, value))
                    _imageHealth.fillAmount = value;
            }
        } 
    }
}

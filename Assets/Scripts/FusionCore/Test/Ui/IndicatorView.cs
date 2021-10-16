using TMPro;
using UnityEngine;

namespace FusionCore.Ui
{
    public class IndicatorView : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _countHealth;
        
        [SerializeField] 
        private TMP_Text _countArmor;

        public void SetDataArmor(float countArmor)
        {
            _countArmor.text = $"{countArmor} armor";
        }
        
        public void SetDataHealth(float countHealth)
        {
            _countHealth.text = $"{countHealth} health";
        }
    }
}

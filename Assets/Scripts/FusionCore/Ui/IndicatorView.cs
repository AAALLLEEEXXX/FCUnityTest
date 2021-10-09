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

        public void SetData(int countHealth, int countArmor)
        {
            _countHealth.text = $"{countHealth} health";
            _countArmor.text = $"{countArmor} armor";
        }
    }
}

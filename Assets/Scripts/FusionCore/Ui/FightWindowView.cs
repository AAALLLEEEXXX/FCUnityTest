using UnityEngine;

namespace FusionCore.Ui
{
    public class FightWindowView : MonoBehaviour
    {
        [SerializeField] 
        private IndicatorView _indicatorPlayer;
        
        [SerializeField] 
        private IndicatorView _indicatorEnemy;

        public IndicatorView IndicatorPlayer => _indicatorPlayer;

        public IndicatorView IndicatorEnemy => _indicatorEnemy;
    }
}

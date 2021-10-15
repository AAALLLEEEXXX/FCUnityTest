using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Data
{
    [CreateAssetMenu(fileName = "CharacterPreset", menuName = "Presets/CharacterPreset", order = 1)]
    public class CharacterPreset : ScriptableObject
    {
        [SerializeField] 
        private CharacterView _characterView;
        
        [SerializeField] 
        private  float _accuracy;
        
        [SerializeField] 
        private  float _dexterity;
        
        [SerializeField] 
        private  float _maxHealth;
        
        [SerializeField] 
        private  float _maxArmor;
        
        [SerializeField] 
        private  float _aimTime;

        public CharacterView CharacterView => _characterView;

        public float Accuracy
        {
            get => _accuracy;
            set => _accuracy = value;
        }

        public float Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        public float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public float MaxArmor
        {
            get => _maxArmor;
            set => _maxArmor = value;
        }

        public float AimTime
        {
            get => _aimTime;
            set => _aimTime = value;
        }
    }
}

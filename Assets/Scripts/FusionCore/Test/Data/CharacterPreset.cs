using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Data
{
    [CreateAssetMenu(fileName = "CharacterPreset", menuName = "Presets/CharacterPreset", order = 1)]
    public class CharacterPreset : ScriptableObject
    {
        [SerializeField] private CharacterView _characterView;

        [SerializeField] private float _accuracy;

        [SerializeField] private float _dexterity;

        [SerializeField] private float _maxHealth;

        [SerializeField] private float _maxArmor;

        [SerializeField] private float _aimTime;

        public CharacterView CharacterView => _characterView;

        public float Accuracy => _accuracy;

        public float Dexterity => _dexterity;

        public float MaxHealth => _maxHealth;

        public float MaxArmor => _maxArmor;

        public float AimTime => _aimTime;
    }
}
using UnityEngine;

namespace FusionCore.Test.Data
{
    [CreateAssetMenu(fileName = "ModifierCharacterPreset", menuName = "Presets/ModifierCharacterPreset", order = 1)]
    public class ModifierCharacterPreset : ScriptableObject
    {
        [SerializeField] [Min(0)] 
        private int _startCountModifier;

        [SerializeField] 
        private CharacterModifier[] _characterModifiers;

        public int StartCountModifier => _startCountModifier;

        public CharacterModifier[] CharacterModifiers => _characterModifiers;
    }
}
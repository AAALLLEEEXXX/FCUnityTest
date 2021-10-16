using UnityEngine;

namespace FusionCore.Test.Data
{
    [CreateAssetMenu(fileName = "ModifierWeaponPreset", menuName = "Presets/ModifierWeaponPreset", order = 1)]
    public class ModifierWeaponPreset : ScriptableObject
    {
        [SerializeField] [Min(0)] 
        private int _startCountModifier;

        [SerializeField] 
        private WeaponModifier[] _weaponModifiers;

        public int StartCountModifier => _startCountModifier;

        public WeaponModifier[] WeaponModifiers => _weaponModifiers;
    }
}
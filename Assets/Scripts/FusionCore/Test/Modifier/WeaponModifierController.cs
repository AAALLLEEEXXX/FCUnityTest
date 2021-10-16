using FusionCore.Test.Data;
using Random = UnityEngine.Random;

namespace FusionCore.Test
{
    public class WeaponModifierController
    {
        public float Accuracy => _weaponPreset.Accuracy + _modifierAccuracy;
        
        public float FireRate => _weaponPreset.FireRate + _modifierFireRate;
        
        public uint ClipSize => _weaponPreset.ClipSize + _modifierClipSize;
        
        public float ReloadTime => _weaponPreset.ReloadTime + _modifierReloadTime;
        
        private readonly WeaponPreset _weaponPreset;
        
        private float _modifierAccuracy;
        private float _modifierFireRate;
        private uint _modifierClipSize;
        private float _modifierReloadTime;
        
        public WeaponModifierController(ModifierWeaponPreset modifierWeaponPresetPreset, WeaponPreset weaponPreset)
        {
            _weaponPreset = weaponPreset;
            
            InitializeModifier(modifierWeaponPresetPreset);
        }

        private void InitializeModifier(ModifierWeaponPreset modifierCharacterPreset)
        {
            var countModifier = modifierCharacterPreset.StartCountModifier;
            
            if (countModifier <= 0)
                return;
            
            for (var i = 0; i < countModifier; i++)
            {
                var randomIndex = Random.Range(0, modifierCharacterPreset.WeaponModifiers.Length);
                ChangeStartSettingsWeapon(modifierCharacterPreset.WeaponModifiers[randomIndex]);
            }
        }

        private void ChangeStartSettingsWeapon(WeaponModifier weaponModifier)
        {
            switch (@weaponModifier.ChangeParameter)
            {
                case WeaponModifierType.Accuracy:
                    _modifierAccuracy = weaponModifier.AddValue;
                    break;
                
                case WeaponModifierType.FireRate:
                    _modifierFireRate = weaponModifier.AddValue;
                    break;
                
                case WeaponModifierType.ClipSize:
                    _modifierClipSize = (uint) weaponModifier.AddValue;
                    break;
                
                case WeaponModifierType.ReloadTime:
                    _modifierReloadTime = weaponModifier.AddValue;
                    break;
            }
        }
    }
}

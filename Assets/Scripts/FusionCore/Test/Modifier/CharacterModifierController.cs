using FusionCore.Test.Data;
using UnityEngine;

namespace FusionCore.Test
{
    public class CharacterModifierController
    {
        private readonly CharacterPreset _characterPreset;

        private float _modifierAccuracy;
        private float _modifierAimTime;
        private float _modifierDexterity;
        private float _modifierMaxArmor;
        private float _modifierMaxHealth;

        public CharacterModifierController(ModifierCharacterPreset modifierCharacterPreset,
            CharacterPreset characterPreset)
        {
            _characterPreset = characterPreset;

            InitializeModifier(modifierCharacterPreset);
        }

        public float Accuracy => _characterPreset.Accuracy + _modifierAccuracy;

        public float Dexterity => _characterPreset.Dexterity + _modifierDexterity;

        public float MaxHealth => _characterPreset.MaxHealth + _modifierMaxHealth;

        public float MaxArmor => _characterPreset.MaxArmor + _modifierMaxArmor;

        public float AimTime => _characterPreset.AimTime + _modifierAimTime;

        private void InitializeModifier(ModifierCharacterPreset modifierCharacterPreset)
        {
            var countModifier = modifierCharacterPreset.StartCountModifier;

            if (countModifier <= 0)
                return;

            for (var i = 0; i < countModifier; i++)
            {
                var randomIndex = Random.Range(0, modifierCharacterPreset.CharacterModifiers.Length);
                ChangeStartSettingsCharacter(modifierCharacterPreset.CharacterModifiers[randomIndex]);
            }
        }

        private void ChangeStartSettingsCharacter(CharacterModifier characterModifier)
        {
            switch (characterModifier.ChangeParameter)
            {
                case CharacterModifierType.Accuracy:
                    _modifierAccuracy = characterModifier.AddValue;
                    break;

                case CharacterModifierType.Dexterity:
                    _modifierDexterity = characterModifier.AddValue;
                    break;

                case CharacterModifierType.MaxHealth:
                    _modifierMaxHealth = characterModifier.AddValue;
                    break;

                case CharacterModifierType.MaxArmor:
                    _modifierMaxArmor = characterModifier.AddValue;
                    break;

                case CharacterModifierType.AimTime:
                    _modifierAimTime = characterModifier.AddValue;
                    break;
            }
        }
    }
}
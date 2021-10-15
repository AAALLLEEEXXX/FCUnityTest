using FusionCore.Test.Data;
using Random = UnityEngine.Random;

namespace FusionCore.Test
{
    public class CharacterModifierController
    {
        public CharacterModifierController(ModifierCharacterPreset modifierCharacterPreset, CharacterPreset characterPreset)
        {
            InitializeModifier(modifierCharacterPreset, characterPreset);
        }

        private void InitializeModifier(ModifierCharacterPreset modifierCharacterPreset, CharacterPreset characterPreset)
        {
            var countModifier = modifierCharacterPreset.StartCountModifier;
            
            if (countModifier <= 0)
                return;
            
            for (var i = 0; i < countModifier; i++)
            {
                var randomIndex = Random.Range(0, modifierCharacterPreset.CharacterModifiers.Length);
                ChangeStartSettingsCharacter(modifierCharacterPreset.CharacterModifiers[randomIndex],
                    characterPreset);
            }
        }

        private void ChangeStartSettingsCharacter(CharacterModifier characterModifier, CharacterPreset characterPreset)
        {
            switch (characterModifier.ChangeParameter)
            {
                case CharacterModifierType.Accuracy:
                    characterPreset.Accuracy += characterModifier.AddValue;
                    break;
                
                case CharacterModifierType.Dexterity:
                    characterPreset.Dexterity += characterModifier.AddValue;
                    break;
                
                case CharacterModifierType.MaxHealth:
                    characterPreset.MaxHealth += characterModifier.AddValue;
                    break;
                
                case CharacterModifierType.MaxArmor:
                    characterPreset.MaxArmor += characterModifier.AddValue;
                    break;
                
                case CharacterModifierType.AimTime:
                    characterPreset.AimTime += characterModifier.AddValue;
                    break;
            }
        }
    }
}

using FusionCore.Test.Generic;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public class CharacterModel : ICharacterModel
    {
        private Team _team;

        public CharacterModel(CharacterModifierController characterModifierController, CharacterView characterView, Team team)
        {
            Team = team;
            CharacterView = characterView;
            Position = characterView.transform.position;

            Health = new SubscriptionProperty<float> {Value = characterModifierController.MaxHealth};
            Armor = new SubscriptionProperty<float> {Value = characterModifierController.MaxArmor};
            
            MaxHealth = characterModifierController.MaxHealth;
            MaxArmor = characterModifierController.MaxArmor;

            AimTime = characterModifierController.AimTime;
            Dexterity = characterModifierController.Dexterity;
            Accuracy = characterModifierController.Accuracy;
            
            CurrentTarget = new SubscriptionProperty<ICharacterModel>();
            PersonState = new SubscriptionProperty<PersonState> {Value = Test.PersonState.Idle};
        }

        public Team Team { get; }
        
        public CharacterView CharacterView { get; }

        public IReadOnlySubscriptionProperty<float> Health { get; }

        public IReadOnlySubscriptionProperty<float> Armor { get; }
        
        public IReadOnlySubscriptionProperty<PersonState> PersonState { get; }
        
        public IReadOnlySubscriptionProperty<ICharacterModel> CurrentTarget { get; }
        
        public float MaxHealth { get; }

        public float MaxArmor { get; }
        
        public float AimTime { get; }
        
        public float Dexterity { get; }
        
        public float Accuracy { get; }

        public bool IsAlive => Health.Value > 0 || Armor.Value > 0;

        public Vector3 Position { get; }
        
        public bool IsHasCurrentTarget => CurrentTarget.Value != null && CurrentTarget.Value.IsAlive;
    }
}
using FusionCore.Test.Data;
using FusionCore.Test.Generic;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public class CharacterModel : ICharacterModel
    {
        private Team _team;

        private CharacterPreset _characterPreset;

        public CharacterModel(CharacterPreset characterPreset, CharacterView characterView, Team team)
        {
            _characterPreset = characterPreset;
            _team = team;
            CharacterView = characterView;
            Position = characterView.transform.position;

            Health = new SubscriptionProperty<float> {Value = _characterPreset.MaxHealth};
            Armor = new SubscriptionProperty<float> {Value = _characterPreset.MaxArmor};
            
            CurrentTarget = new SubscriptionProperty<CharacterModel>();
            PersonState = new SubscriptionProperty<PersonState> {Value = Test.PersonState.Idle};
        }

        public Team Team => _team;
        
        public CharacterView CharacterView { get; }

        public IReadOnlySubscriptionProperty<float> Health { get; }

        public IReadOnlySubscriptionProperty<float> Armor { get; }
        
        public IReadOnlySubscriptionProperty<PersonState> PersonState { get; }

        public float AimTime => _characterPreset.AimTime;
        
        public float Dexterity => _characterPreset.Dexterity;
        
        public float Accuracy => _characterPreset.Accuracy;

        public bool IsAlive => Health.Value > 0 || Armor.Value > 0;

        public Vector3 Position { get; }

        public IReadOnlySubscriptionProperty<CharacterModel> CurrentTarget { get; }

        public bool IsHasCurrentTarget => CurrentTarget.Value != null && CurrentTarget.Value.IsAlive;
    }
}
using FusionCore.Test.Data;
using FusionCore.Test.Views;
using UniRx;
using UnityEngine;

namespace FusionCore.Test
{
    public class CharacterModel : ICharacterModel
    {
        private Team _team;

        private CharacterPreset _characterPreset;
        
        public CharacterModel(CharacterPreset characterPreset, Team team)
        {
            _characterPreset = characterPreset;
            _team = team;

            Health.Value = _characterPreset.MaxHealth;
            Health.Value = _characterPreset.MaxArmor;
            PersonState.Value = Test.PersonState.Idle;
        }

        public Team Team => _team;
        
        public CharacterView CharacterView => _characterPreset.CharacterView;

        public IReactiveProperty<float> Health { get; } = new ReactiveProperty<float>();

        public IReactiveProperty<float> Armor { get; } = new ReactiveProperty<float>();
        
        public IReactiveProperty<PersonState> PersonState { get; } = new ReactiveProperty<PersonState>();

        public float AimTime => _characterPreset.AimTime;
        
        public float Dexterity => _characterPreset.Dexterity;
        
        public float Accuracy => _characterPreset.Accuracy;

        public bool IsAlive => Health.Value > 0 || Armor.Value > 0;

        public Vector3 Position => _characterPreset.CharacterView.transform.position;

        public IReactiveProperty<CharacterModel> CurrentTarget { get; } = new ReactiveProperty<CharacterModel>();
    }
}
using FusionCore.Test.Generic;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public interface ICharacterModel
    {
        public Team Team { get; }
        
        public CharacterView CharacterView { get; }
        
        public IReadOnlySubscriptionProperty<float> Health { get; }
        
        public IReadOnlySubscriptionProperty<float> Armor { get; }
        
        public bool IsAlive { get; }
        
        public Vector3 Position { get; }
        
        public float AimTime { get; }
        
        public float Dexterity { get; }
        
        public float Accuracy { get; }
        
        public IReadOnlySubscriptionProperty<CharacterModel> CurrentTarget { get; }
        
        public IReadOnlySubscriptionProperty<PersonState> PersonState { get; }
        
        public bool IsHasCurrentTarget { get; }
    }
}

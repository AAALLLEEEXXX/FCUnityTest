using FusionCore.Test.Views;
using UniRx;
using UnityEngine;

namespace FusionCore.Test
{
    public interface ICharacterModel
    {
        public Team Team { get; }
        
        public CharacterView CharacterView { get; }
        
        public IReactiveProperty<float> Health { get; }
        
        public IReactiveProperty<float> Armor { get; }
        
        public bool IsAlive { get; }
        
        public Vector3 Position { get; }
        
        public float AimTime { get; }
        
        public float Dexterity { get; }
        
        public float Accuracy { get; }
        
        public IReactiveProperty<CharacterModel> CurrentTarget { get; }
        
        public IReactiveProperty<PersonState> PersonState { get; }
    }
}

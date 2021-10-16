using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class AimingCharacterState : BaseCharacterState
    {
        private readonly ICharacterModel _model;
        
        public AimingCharacterState(ICharacterModel model)
        {
            _model = model;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, false);

            if (!_model.IsHasCurrentTarget)
            {
                _model.PersonState.Value = PersonState.Idle;
                return;
            }
            
            if (_time > 0)
            {
                _time -= Time.deltaTime;
                return;
            }

            _model.PersonState.Value = PersonState.Shooting;
            _time = 0;
        }
    }
}

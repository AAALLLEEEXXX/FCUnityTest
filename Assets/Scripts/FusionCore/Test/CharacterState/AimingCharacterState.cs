using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class AimingCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        
        public AimingCharacterState(CharacterModel model)
        {
            _model = model;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, false);
						
            if (_model.CurrentTarget.Value != null && _model.CurrentTarget.Value.IsAlive)
            {
                if (_time > 0)
                {
                    _time -= Time.deltaTime;
                }
                else
                {
                    _model.PersonState.Value = PersonState.Shooting;
                    _time = 0;
                }
            }
            else
            {
                _model.PersonState.Value = PersonState.Idle;
                _time = 0;
            }
        }
    }
}

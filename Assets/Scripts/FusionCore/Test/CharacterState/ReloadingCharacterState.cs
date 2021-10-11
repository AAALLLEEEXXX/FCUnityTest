using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ReloadingCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        private readonly Weapon _weapon;
        
        public ReloadingCharacterState(CharacterModel model, Weapon weapon)
        {
            _model = model;
            _weapon = weapon;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, true);
            _model.CharacterView.Animator.SetFloat(ReloadTime, _weapon.WeaponView.WeaponDescriptor.ReloadTime / 3.3f);
						
            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
            else
            {
                if (_model.CurrentTarget.HasValue && _model.CurrentTarget.Value.IsAlive)
                    _model.PersonState.Value = PersonState.Shooting;
                else
                    _model.PersonState.Value = PersonState.Idle;
				
                _weapon.Reload();
                _time = 0;
            }
        }
    }
}

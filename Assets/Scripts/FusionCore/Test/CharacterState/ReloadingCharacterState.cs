using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ReloadingCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        private readonly WeaponController _weaponController;
        
        public ReloadingCharacterState(CharacterModel model, WeaponController weaponController)
        {
            _model = model;
            _weaponController = weaponController;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, true);
            _model.CharacterView.Animator.SetFloat(ReloadTime, _weaponController.WeaponView.WeaponPreset.ReloadTime);
						
            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
            else
            {
                if (_model.CurrentTarget.Value != null && _model.CurrentTarget.Value.IsAlive)
                    _model.PersonState.Value = PersonState.Shooting;
                else
                    _model.PersonState.Value = PersonState.Idle;
				
                _weaponController.Reload();
                _time = 0;
            }
        }
    }
}

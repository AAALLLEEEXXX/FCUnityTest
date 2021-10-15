using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ShootingCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        private readonly WeaponController _weaponController;
        
        public ShootingCharacterState(CharacterModel model, WeaponController weaponController)
        {
            _model = model;
            _weaponController = weaponController;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, false);
			
            if (_model.CurrentTarget.Value != null && _model.CurrentTarget.Value.IsAlive)
            {
                if (_weaponController.HasAmmo)
                {
                    if (_weaponController.IsReady)
                    {
                        var random = Random.Range(0.0f, 1.0f);
                        var hit = random <= _model.Accuracy &&
                                  random <= _weaponController.WeaponView.WeaponPreset.Accuracy &&
                                  random >= _model.CurrentTarget.Value.Dexterity;
                        
                        _weaponController.Fire(_model.CurrentTarget.Value, hit);
                        _model.CharacterView.Animator.SetTrigger(Shoot);
                    }
                    else
                    {
                        _weaponController.Update();
                    }
                }
                else
                {
                    _model.PersonState.Value = PersonState.Reloading;
                    _time = _weaponController.WeaponView.WeaponPreset.ReloadTime;
                }
            }
            else
            {
                _model.PersonState.Value = PersonState.Idle;
            }
        }
    }
}

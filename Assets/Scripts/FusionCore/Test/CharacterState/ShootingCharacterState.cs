using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ShootingCharacterState : BaseCharacterState
    {
        private readonly CharacterModel _model;
        private readonly Weapon _weapon;
        
        public ShootingCharacterState(CharacterModel model, Weapon weapon)
        {
            _model = model;
            _weapon = weapon;
        }
        
        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, false);
			
            if (_model.CurrentTarget.HasValue && _model.CurrentTarget.Value.IsAlive)
            {
                if (_weapon.HasAmmo)
                {
                    if (_weapon.IsReady)
                    {
                        var random = Random.Range(0.0f, 1.0f);
                        var hit = random <= _model.Accuracy &&
                                  random <= _weapon.WeaponView.WeaponPreset.Accuracy &&
                                  random >= _model.CurrentTarget.Value.Dexterity;
                        
                        _weapon.Fire(_model.CurrentTarget.Value, hit);
                        _model.CharacterView.Animator.SetTrigger(Shoot);
                    }
                    else
                    {
                        _weapon.Update();
                    }
                }
                else
                {
                    _model.PersonState.Value = PersonState.Reloading;
                    _time = _weapon.WeaponView.WeaponPreset.ReloadTime;
                }
            }
            else
            {
                _model.PersonState.Value = PersonState.Idle;
            }
        }
    }
}

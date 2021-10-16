using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ShootingCharacterState : BaseCharacterState
    {
        private readonly ICharacterModel _model;
        private readonly WeaponController _weaponController;

        public ShootingCharacterState(ICharacterModel model, WeaponController weaponController)
        {
            _model = model;
            _weaponController = weaponController;
        }

        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, false);

            if (_model.IsHasCurrentTarget)
            {
                if (_weaponController.HasAmmo)
                    Fire();
                else
                    _model.PersonState.Value = PersonState.Reloading;
            }
            else
            {
                _model.PersonState.Value = PersonState.Idle;
            }
        }

        private void Fire()
        {
            if (_weaponController.IsReady)
            {
                var hit = IsHitTarget();

                _weaponController.Fire(_model.CurrentTarget.Value, hit);
                _model.CharacterView.Animator.SetTrigger(Shoot);
            }
            else
            {
                _weaponController.Update();
            }
        }

        private bool IsHitTarget()
        {
            var random = Random.Range(0.0f, 1.0f);
            return random <= _model.Accuracy &&
                   random <= _weaponController.WeaponModifierController.Accuracy &&
                   random >= _model.CurrentTarget.Value.Dexterity;
        }
    }
}
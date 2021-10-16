using FusionCore.Test.Models;
using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public class ReloadingCharacterState : BaseCharacterState
    {
        private readonly ICharacterModel _model;
        private readonly WeaponController _weaponController;

        public ReloadingCharacterState(ICharacterModel model, WeaponController weaponController)
        {
            _model = model;
            _weaponController = weaponController;
        }

        public override void Something()
        {
            _model.CharacterView.Animator.SetBool(Aiming, true);
            _model.CharacterView.Animator.SetBool(Reloading, true);
            _model.CharacterView.Animator.SetFloat(ReloadTime, _weaponController.WeaponModifierController.ReloadTime);

            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
            else
            {
                _model.PersonState.Value = _model.IsHasCurrentTarget ? PersonState.Shooting : PersonState.Idle;

                _weaponController.Reload();
                _time = 0;
            }
        }
    }
}
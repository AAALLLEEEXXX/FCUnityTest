using System;
using FusionCore.Test.CharacterState;

namespace FusionCore.Test.Models
{
    public class Character : IDisposable
    {
        private readonly FightService _fightService;

        private BaseCharacterState _baseCharacterState;

        private PersonState _personState;
        private readonly WeaponController _weaponController;

        public Character(ICharacterModel model, WeaponController weaponController, FightService fightService)
        {
            Model = model;
            _weaponController = weaponController;
            _fightService = fightService;

            Model.PersonState.Value = PersonState.Idle;
            Model.PersonState.SubscribeOnChange(ChangePersonState);
        }

        public ICharacterModel Model { get; }

        public void Dispose()
        {
            Model.PersonState.UnSubscriptionOnChange(ChangePersonState);
        }

        public void Update()
        {
            if (!Model.IsAlive)
                return;

            switch (_personState)
            {
                case PersonState.Idle:
                    _baseCharacterState = new IdleCharacterState(Model, _fightService);
                    break;

                case PersonState.Aiming:
                    _baseCharacterState = new AimingCharacterState(Model);
                    break;

                case PersonState.Shooting:
                    _baseCharacterState = new ShootingCharacterState(Model, _weaponController);
                    break;

                case PersonState.Reloading:
                    _baseCharacterState = new ReloadingCharacterState(Model, _weaponController);
                    break;
            }

            _baseCharacterState.Something();
        }

        private void ChangePersonState(PersonState personState)
        {
            _personState = personState;
        }
    }
}
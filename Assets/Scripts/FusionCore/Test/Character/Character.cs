using System;
using FusionCore.Test.CharacterState;

namespace FusionCore.Test.Models
{
	public class Character : IDisposable
	{
		private WeaponController _weaponController;

		private PersonState _personState;

		private readonly ICharacterModel _model;
		private readonly FightService _fightService;
		
		private BaseCharacterState _baseCharacterState;
		
		public ICharacterModel Model => _model;

		public Character(ICharacterModel model, WeaponController weaponController, FightService fightService)
		{
			_model = model;
			_weaponController = weaponController;
			_fightService = fightService;

			_model.PersonState.Value = PersonState.Idle;
			_model.PersonState.SubscribeOnChange(ChangePersonState);
		}

		public void Update()
		{
			if (!_model.IsAlive) 
				return;
			
			switch (_personState)
			{
				case PersonState.Idle:
					_baseCharacterState = new IdleCharacterState(_model, _fightService);
					break;
					
				case PersonState.Aiming:
					_baseCharacterState = new AimingCharacterState(_model);
					break;
					
				case PersonState.Shooting:
					_baseCharacterState = new ShootingCharacterState(_model, _weaponController);
					break;
					
				case PersonState.Reloading:
					_baseCharacterState = new ReloadingCharacterState(_model, _weaponController);
					break;
			}
				
			_baseCharacterState.Something();
		}
		
		private void ChangePersonState(PersonState personState)
		{
			_personState = personState;
		}

		public void Dispose()
		{
			_model.PersonState.UnSubscriptionOnChange(ChangePersonState);
		}
	}
}
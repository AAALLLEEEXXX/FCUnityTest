using System;
using FusionCore.Test.CharacterState;
using UniRx;

namespace FusionCore.Test.Models
{
	public class Character : IDisposable
	{
		private Weapon _weapon;

		private PersonState _personState;
		private Character _currentTarget;

		private CharacterModel _model;
		private BaseCharacterState _baseCharacterState;

		private readonly CompositeDisposable _disposables = new CompositeDisposable();
		
		public CharacterModel Model => _model;

		public Character(CharacterModel model, Weapon weapon)
		{
			_model = model;
			_weapon = weapon;

			_model.PersonState.Value = PersonState.Idle;
			_model.PersonState.Subscribe(ChangePersonState).AddTo(_disposables);
		}

		public void Update()
		{
			if (!_model.IsAlive) 
				return;
			
			switch (_personState)
			{
				case PersonState.Idle:
					_baseCharacterState = new IdleCharacterState(_model);
					break;
					
				case PersonState.Aiming:
					_baseCharacterState = new AimingCharacterState(_model);
					break;
					
				case PersonState.Shooting:
					_baseCharacterState = new ShootingCharacterState(_model, _weapon);
					break;
					
				case PersonState.Reloading:
					_baseCharacterState = new ReloadingCharacterState(_model, _weapon);
					break;
			}
				
			_baseCharacterState.Something();
		}

		private void ChangeCurrentTarget(Character target)
		{
			_currentTarget = target;
		}
		
		private void ChangePersonState(PersonState personState)
		{
			_personState = personState;
		}

		public void Dispose()
		{
			_disposables.Clear();
		}
	}
}
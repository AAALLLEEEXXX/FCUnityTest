using System;
using FusionCore.Test.CharacterState;
using UniRx;

namespace FusionCore.Test.Models
{
	public class Character : IDisposable
	{
		private Weapon _weapon;

		private PersonState _personState;

		private CharacterModel _model;
		private BaseCharacterState _baseCharacterState;
		private FightService _fightService;
		private GameModel _gameModel;

		private readonly CompositeDisposable _disposables = new CompositeDisposable();
		
		public CharacterModel Model => _model;

		public Character(CharacterModel model, Weapon weapon, FightService fightService, GameModel gameModel)
		{
			_model = model;
			_weapon = weapon;
			_fightService = fightService;
			_gameModel = gameModel;

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
					if (_fightService.TryGetNearestAliveEnemyNew(_model, out var target))
						_model.CurrentTarget.Value = target;

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
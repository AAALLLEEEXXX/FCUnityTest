using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class Character
	{
		private readonly CharacterView _characterView;
		private readonly Weapon _weapon;

		private PersonState _personState;
		private Character _currentTarget;
		private float _time;
		
		public CharacterModel Model => _model;
		
		private CharacterModel _model;
		
		private static readonly int Aiming = Animator.StringToHash("aiming");
		private static readonly int Reloading = Animator.StringToHash("reloading");
		private static readonly int ReloadTime = Animator.StringToHash("reload_time");
		private static readonly int Shoot = Animator.StringToHash("shoot");

		public Character(CharacterModel model, Weapon weapon)
		{
			_model = model;
			_weapon = weapon;
		}

		public CharacterView CharacterView => _characterView;
		public Vector3 Position => _characterView.transform.position;

		public void Update()
		{
			if (_model.IsAlive)
			{
				switch (_personState)
				{
					case PersonState.Idle:
						_characterView.Animator.SetBool(Aiming, false);
						_characterView.Animator.SetBool(Reloading, false);
						
						//_currentTarget = target;
						_personState = PersonState.Aiming;
						_time = _model.AimTime;
						_characterView.transform.LookAt(_model.CurrentTarget.Value.Position);

						break;
					
					case PersonState.Aiming:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, false);
						
						if (_model.CurrentTarget.HasValue && _model.CurrentTarget.Value.IsAlive)
						{
							if (_time > 0)
							{
								_time -= Time.deltaTime;
							}
							else
							{
								_personState = PersonState.Shooting;
								_time = 0;
							}
						}
						else
						{
							_personState = PersonState.Idle;
							_time = 0;
						}
						break;
					
					case PersonState.Shooting:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, false);
						if (_model.CurrentTarget.HasValue && _model.CurrentTarget.Value.IsAlive)
						{
							if (_weapon.HasAmmo)
							{
								if (_weapon.IsReady)
								{
									var random = Random.Range(0.0f, 1.0f);
									var hit = random <= _model.Accuracy &&
									          random <= _weapon.WeaponView.WeaponDescriptor.Accuracy &&
									          random >= _model.CurrentTarget.Value.Dexterity;
									_weapon.Fire(_currentTarget, hit);
									_characterView.Animator.SetTrigger(Shoot);
								}
								else
								{
									_weapon.Update();
								}
							}
							else
							{
								_personState = PersonState.Reloading;
								_time = _weapon.WeaponView.WeaponDescriptor.ReloadTime;
							}
						}
						else
						{
							_personState = PersonState.Idle;
						}
						break;
					
					case PersonState.Reloading:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, true);
						_characterView.Animator.SetFloat(ReloadTime, _weapon.WeaponView.WeaponDescriptor.ReloadTime / 3.3f);
						
						if (_time > 0)
						{
							_time -= Time.deltaTime;
						}
						else
						{
							if (_model.CurrentTarget.HasValue && _model.CurrentTarget.Value.IsAlive)
							{
								_personState = PersonState.Shooting;
							}
							else
							{
								_personState = PersonState.Idle;
							}
							_weapon.Reload();
							_time = 0;
						}
						break;
				}
			}
		}
	}
}
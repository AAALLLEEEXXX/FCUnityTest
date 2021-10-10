using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class Character
	{
		private enum State
		{
			Idle,
			Aiming,
			Shooting,
			Reloading
		}

		private readonly CharacterView _characterView;
		private readonly Weapon _weapon;
		private readonly FightController _fightController;

		private float _health;
		private float _armor;

		private State _state;
		private Character _currentTarget;
		private float _time;
		
		private static readonly int Aiming = Animator.StringToHash("aiming");
		private static readonly int Reloading = Animator.StringToHash("reloading");
		private static readonly int ReloadTime = Animator.StringToHash("reload_time");
		private static readonly int Shoot = Animator.StringToHash("shoot");

		public Character(CharacterView characterView, Weapon weapon, FightController fightController)
		{
			_characterView = characterView;
			_weapon = weapon;
			_fightController = fightController;
			_health = _characterView.CharacterDescriptor.MaxHealth;
			_armor = _characterView.CharacterDescriptor.MaxArmor;
		}

		public bool IsAlive => _health > 0 || _armor > 0;

		public float Health
		{
			get => _health;
			set => _health = value;
		}

		public float Armor
		{
			get => _armor;
			set => _armor = value;
		}

		public CharacterView CharacterView => _characterView;
		public Vector3 Position => _characterView.transform.position;

		public void Update(float deltaTime)
		{
			if (IsAlive)
			{
				switch (_state)
				{
					case State.Idle:
						_characterView.Animator.SetBool(Aiming, false);
						_characterView.Animator.SetBool(Reloading, false);
						if (_fightController.TryGetNearestAliveEnemy(this, out Character target))
						{
							_currentTarget = target;
							_state = State.Aiming;
							_time = _characterView.CharacterDescriptor.AimTime;
							_characterView.transform.LookAt(_currentTarget.Position);
						}
						break;
					
					case State.Aiming:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, false);
						if (_currentTarget != null && _currentTarget.IsAlive)
						{
							if (_time > 0)
							{
								_time -= deltaTime;
							}
							else
							{
								_state = State.Shooting;
								_time = 0;
							}
						}
						else
						{
							_state = State.Idle;
							_time = 0;
						}
						break;
					
					case State.Shooting:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, false);
						if (_currentTarget != null && _currentTarget.IsAlive)
						{
							if (_weapon.HasAmmo)
							{
								if (_weapon.IsReady)
								{
									float random = Random.Range(0.0f, 1.0f);
									bool hit = random <= _characterView.CharacterDescriptor.Accuracy &&
											random <= _weapon.WeaponView.WeaponDescriptor.Accuracy &&
											random >= _currentTarget.CharacterView.CharacterDescriptor.Dexterity;
									_weapon.Fire(_currentTarget, hit);
									_characterView.Animator.SetTrigger(Shoot);
								}
								else
								{
									_weapon.Update(deltaTime);
								}
							}
							else
							{
								_state = State.Reloading;
								_time = _weapon.WeaponView.WeaponDescriptor.ReloadTime;
							}
						}
						else
						{
							_state = State.Idle;
						}
						break;
					
					case State.Reloading:
						_characterView.Animator.SetBool(Aiming, true);
						_characterView.Animator.SetBool(Reloading, true);
						_characterView.Animator.SetFloat(ReloadTime, _weapon.WeaponView.WeaponDescriptor.ReloadTime / 3.3f);
						if (_time > 0)
						{
							_time -= deltaTime;
						}
						else
						{
							if (_currentTarget != null && _currentTarget.IsAlive)
							{
								_state = State.Shooting;
							}
							else
							{
								_state = State.Idle;
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
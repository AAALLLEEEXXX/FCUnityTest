using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class Weapon
	{
		private WeaponView _weaponView;

		private uint _ammo;

		private bool _ready;
		private float _time;

		public Weapon(WeaponView weaponView)
		{
			_weaponView = weaponView;
			_ammo = _weaponView.WeaponPreset.ClipSize;
		}

		public bool IsReady => _ready;
		
		public bool HasAmmo => _ammo > 0;

		public WeaponView WeaponView => _weaponView;

		public void Reload()
		{
			_ammo = _weaponView.WeaponPreset.ClipSize;
		}

		public void Fire(CharacterModel character, bool hit)
		{
			if (_ammo > 0)
			{
				_ammo -= 1;
				var bullet = Object.Instantiate(_weaponView.BulletPrefab, _weaponView.BarrelTransform);
				bullet.Init(_weaponView, character, hit);
				_time = 1.0f / _weaponView.WeaponPreset.FireRate;
				_ready = false;
			}
		}

		public void Update()
		{
			if (_ready) 
				return;
			
			if (_time > 0)
				_time -= Time.deltaTime;
			else
				_ready = true;
		}
	}
}
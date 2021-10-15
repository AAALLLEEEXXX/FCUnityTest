using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class WeaponController
	{
		private WeaponView _weaponView;
		private BulletController _bulletController;

		private uint _ammo;

		private bool _ready;
		private float _time;

		public WeaponController(WeaponView weaponView)
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
			CreateBullet(character, hit);
			_ammo--;
			_time = 1.0f / _weaponView.WeaponPreset.FireRate;
			_ready = false;
		}

		public void Update()
		{
			if (_ready) 
				return;

			_bulletController?.Update();

			if (_time > 0)
				_time -= Time.deltaTime;
			else
				_ready = true;
		}

		private void CreateBullet(CharacterModel character, bool hit)
		{
			var bullet = Object.Instantiate(_weaponView.BulletPrefab, _weaponView.BarrelTransform);
			_bulletController = new BulletController(bullet, _weaponView, character, hit);
		}
	}
}
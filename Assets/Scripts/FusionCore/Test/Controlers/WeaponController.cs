using FusionCore.Test.Data;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class WeaponController
	{
		public WeaponModifierController WeaponModifierController { get; }
		
		private WeaponView _weaponView;
		private BulletController _bulletController;
		
		private uint _ammo;

		private bool _ready;
		private float _time;

		public WeaponController(WeaponView weaponView, ModifierWeaponPreset modifierWeaponPreset)
		{
			WeaponModifierController = new WeaponModifierController(modifierWeaponPreset, weaponView.WeaponPreset);
			
			_weaponView = weaponView;
			_ammo = WeaponModifierController.ClipSize;
		}

		public bool IsReady => _ready;
		
		public bool HasAmmo => _ammo > 0;

		public void Reload()
		{
			_ammo = WeaponModifierController.ClipSize;
		}

		public void Fire(ICharacterModel character, bool hit)
		{
			CreateBullet(character, hit);
			_ammo--;
			_time = 1.0f / WeaponModifierController.FireRate;
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

		private void CreateBullet(ICharacterModel character, bool hit)
		{
			var bullet = Object.Instantiate(_weaponView.BulletPrefab, _weaponView.BarrelTransform);
			_bulletController = new BulletController(bullet, _weaponView, character, hit);
		}
	}
}
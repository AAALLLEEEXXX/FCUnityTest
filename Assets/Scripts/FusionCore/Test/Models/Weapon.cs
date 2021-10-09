using FusionCore.Test.Descriptors;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
	public class Weapon
	{
		private readonly WeaponPrefab _prefab;

		private uint _ammo;

		private bool _ready;
		private float _time;

		public Weapon(WeaponPrefab prefab)
		{
			_prefab = prefab;
			_ammo = _prefab.GetComponent<WeaponDescriptor>().ClipSize;
		}

		public bool IsReady => _ready;
		public bool HasAmmo => _ammo > 0;

		public WeaponPrefab Prefab => _prefab;

		public void Reload()
		{
			_ammo = _prefab.GetComponent<WeaponDescriptor>().ClipSize;
		}

		public void Fire(Character character, bool hit)
		{
			if (_ammo > 0)
			{
				_ammo -= 1;
				FireBullet(character, hit);
				_time = 1.0f / _prefab.GetComponent<WeaponDescriptor>().FireRate;
				_ready = false;
			}
		}
		
		
		private void FireBullet(Character character, bool hit)
		{
			var bullet = Object.Instantiate(_prefab.BulletPrefab, _prefab.BarrelTransform);
			bullet.transform.position = _prefab.BarrelTransform.position;
			bullet.Init(_prefab, character, hit);
		}

		public void Update(float deltaTime)
		{
			if (_ready) 
				return;
			
			if (_time > 0)
			{
				_time -= deltaTime;
			}
			else
			{
				_ready = true;
			}
		}
	}
}
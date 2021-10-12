using FusionCore.Test.Data;
using FusionCore.Test.Descriptors;
using UnityEngine;

namespace FusionCore.Test.Views
{
	public class WeaponView : MonoBehaviour
	{
		[SerializeField]
		private Transform _barrelTransform;

		[SerializeField]
		private BulletPrefab _bulletPrefab;

		[SerializeField] 
		private WeaponPreset _weaponPreset;

		public Transform BarrelTransform => _barrelTransform;
		
		public BulletPrefab BulletPrefab => _bulletPrefab;
		
		public WeaponPreset WeaponPreset => _weaponPreset;
	}
}
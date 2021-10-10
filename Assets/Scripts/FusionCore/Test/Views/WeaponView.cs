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
		private WeaponDescriptor _weaponDescriptor;

		public Transform BarrelTransform => _barrelTransform;
		
		public BulletPrefab BulletPrefab => _bulletPrefab;
		
		public WeaponDescriptor WeaponDescriptor => _weaponDescriptor;
	}
}
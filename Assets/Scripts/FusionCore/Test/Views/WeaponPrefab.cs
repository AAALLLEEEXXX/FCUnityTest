using UnityEngine;

namespace FusionCore.Test.Views
{
	public class WeaponPrefab : MonoBehaviour
	{
		[SerializeField]
		private Transform _barrelTransform;

		[SerializeField]
		private BulletPrefab _bulletPrefab;

		public Transform BarrelTransform => _barrelTransform;
		
		public BulletPrefab BulletPrefab => _bulletPrefab;
	}
}
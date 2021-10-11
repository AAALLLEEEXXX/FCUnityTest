using UnityEngine;

namespace FusionCore.Test.Views
{
	public class CharacterView : MonoBehaviour
	{
		[SerializeField]
		private WeaponView _weaponView;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private Transform _rightPalm;

		public Animator Animator => _animator;
		
		public WeaponView WeaponView => _weaponView;

		public void Update()
		{
			_weaponView.transform.position = _rightPalm.position;
			_weaponView.transform.forward = _rightPalm.up;
		}
	}
}

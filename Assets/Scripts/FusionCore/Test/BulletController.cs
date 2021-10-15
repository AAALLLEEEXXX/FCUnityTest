using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public class BulletController
    {
        private BulletPrefab _bulletPrefab;
        
        public BulletController(BulletPrefab bulletPrefab, WeaponView weapon, CharacterModel target, bool hit)
        {
            _bulletPrefab = bulletPrefab;
            _weapon = weapon;
            _target = target;
            _hit = hit;
            _position = _bulletPrefab.transform.position;
        	
            var targetPosition = target.Position + Vector3.up * 2.0f;
            _direction = Vector3.Normalize(targetPosition - _bulletPrefab.transform.position);
            _totalDistance = Vector3.Distance(targetPosition, _bulletPrefab.transform.position);
            _currentDistance = 0;
        }
        
        private CharacterModel _target;
        private WeaponView _weapon;
        private bool _hit;

        private Vector3 _position;
        private Vector3 _direction;
        private float _totalDistance;
        private float _currentDistance;
        
        private static readonly int Die = Animator.StringToHash("die");

        public void Update()
        {
        	_currentDistance += Time.deltaTime * 30;
        	if (_currentDistance < _totalDistance)
        	{
                _bulletPrefab.transform.position = _position + _currentDistance * _direction;
        	}
        	else
        	{
        		if (_hit)
        		{
        			var weaponDescriptor = _weapon.WeaponPreset;
        			var damage = weaponDescriptor.Damage;
        			
        			if (_target.Armor.Value > 0)
        				_target.Armor.Value -= damage;
        			else if (_target.Health.Value > 0)
        				_target.Health.Value -= damage;
        			if (_target.Armor.Value <= 0 && _target.Health.Value <= 0)
        				_target.CharacterView.Animator.SetTrigger(Die);
        		}
                
        		Object.Destroy(_bulletPrefab);
        	}
        }
    }
}

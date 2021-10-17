using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
    public class BulletController
    {
        private static readonly int Die = Animator.StringToHash("die");

        private readonly BulletPrefab _bulletPrefab;
        private readonly bool _hit;
        private readonly ICharacterModel _target;
        private readonly WeaponView _weapon;
        private readonly Vector3 _direction;
        private readonly Vector3 _position;
        private readonly float _totalDistance;
        
        private float _currentDistance;
        private bool _isDestroyBullet;

        public BulletController(BulletPrefab bulletPrefab, WeaponView weapon, ICharacterModel target, bool hit)
        {
            _bulletPrefab = bulletPrefab;
            _weapon = weapon;
            _target = target;
            _hit = hit;
            _position = bulletPrefab.transform.position;

            var targetPosition = target.Position + Vector3.up * 2.0f;
            _direction = Vector3.Normalize(targetPosition - bulletPrefab.transform.position);
            _totalDistance = Vector3.Distance(targetPosition, weapon.BarrelTransform.transform.position) - 0.3f;
        }

        public void Update()
        {
            if (_isDestroyBullet)
                return;
            
            _currentDistance += Time.deltaTime * _bulletPrefab.Speed;

            if (_currentDistance < _totalDistance)
            {
                _bulletPrefab.transform.position = _position + _currentDistance * _direction;
            }
            else
            {
                if (_hit)
                {
                    var damage = _weapon.WeaponPreset.Damage;

                    if (_target.Armor.Value > 0)
                        _target.Armor.Value -= damage;
                    else if (_target.Health.Value > 0)
                        _target.Health.Value -= damage;

                    if (_target.Armor.Value <= 0)
                        _target.Armor.Value = 0;

                    if (_target.Health.Value <= 0)
                    {
                        _target.Health.Value = 0;
                        _target.CharacterView.Animator.SetTrigger(Die);
                    }
                }

                Object.Destroy(_bulletPrefab.gameObject);
                _isDestroyBullet = true;
            }
        }
    }
}
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
        private float _currentDistance;
        private readonly Vector3 _direction;

        private readonly Vector3 _position;
        private readonly float _totalDistance;

        public BulletController(BulletPrefab bulletPrefab, WeaponView weapon, ICharacterModel target, bool hit)
        {
            _bulletPrefab = bulletPrefab;
            _weapon = weapon;
            _target = target;
            _hit = hit;
            _position = bulletPrefab.transform.position;

            var targetPosition = target.Position + Vector3.up * 2.0f;
            _direction = Vector3.Normalize(targetPosition - _bulletPrefab.transform.position);
            _totalDistance = Vector3.Distance(targetPosition, _bulletPrefab.transform.position);
        }

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

                Object.Destroy(_bulletPrefab);
            }
        }
    }
}
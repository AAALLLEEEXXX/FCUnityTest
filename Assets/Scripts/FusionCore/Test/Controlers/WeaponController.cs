using System.Collections.Generic;
using FusionCore.Test.Data;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test.Models
{
    public class WeaponController
    {
        private uint _ammo;
        private List<BulletController> _bulletControllers = new List<BulletController>();

        private float _time;

        private readonly WeaponView _weaponView;

        public WeaponController(WeaponView weaponView, ModifierWeaponPreset modifierWeaponPreset)
        {
            WeaponModifierController = new WeaponModifierController(modifierWeaponPreset, weaponView.WeaponPreset);

            _weaponView = weaponView;
            _ammo = WeaponModifierController.ClipSize;
        }

        public WeaponModifierController WeaponModifierController { get; }

        public bool IsReady { get; private set; }

        public bool HasAmmo => _ammo > 0;

        public void Reload()
        {
            _ammo = WeaponModifierController.ClipSize;
            _bulletControllers.Clear();
        }

        public void Fire(ICharacterModel character, bool hit)
        {
            if (!HasAmmo)
                return;
            
            CreateBullet(character, hit);
            _ammo--;
            _time = 1.0f / WeaponModifierController.FireRate;
            IsReady = false;
        }

        public void Update()
        {
            foreach (var bulletController in _bulletControllers)
                bulletController?.Update();
            
            if (IsReady)
                return;
            
            if (_time > 0)
                _time -= Time.deltaTime;
            else
                IsReady = true;
        }

        private void CreateBullet(ICharacterModel character, bool hit)
        {
            var bullet = Object.Instantiate(_weaponView.BulletPrefab, _weaponView.BarrelTransform);
            var bulletController = new BulletController(bullet, _weaponView, character, hit);
            _bulletControllers.Add(bulletController);
        }
    }
}
using UnityEngine;

namespace FusionCore.Test.Data
{
    [CreateAssetMenu(fileName = "WeaponPreset", menuName = "Presets/WeaponPreset", order = 1)]
    public class WeaponPreset : ScriptableObject
    {
        private const float CoefReloadTime = 3.3f;
        
        [SerializeField] private float _damage;

        [SerializeField] private float _accuracy;

        [SerializeField] private float _fireRate;

        [SerializeField] private uint _clipSize;

        [SerializeField] private float _reloadTime;

        public float Damage => _damage;

        public float Accuracy => _accuracy;

        public float FireRate => _fireRate;

        public uint ClipSize => _clipSize;

        public float ReloadTime => _reloadTime / CoefReloadTime;
    }
}
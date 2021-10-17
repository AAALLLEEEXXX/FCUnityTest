using UnityEngine;

namespace FusionCore.Test.Views
{
    public class BulletPrefab : MonoBehaviour
    {
        [SerializeField] 
        private float _speed;

        public float Speed => _speed;
    }
}
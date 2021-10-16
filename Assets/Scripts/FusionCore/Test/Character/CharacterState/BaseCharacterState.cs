using UnityEngine;

namespace FusionCore.Test.CharacterState
{
    public abstract class BaseCharacterState
    {
        protected static readonly int Aiming = Animator.StringToHash("aiming");
        protected static readonly int Reloading = Animator.StringToHash("reloading");
        protected static readonly int ReloadTime = Animator.StringToHash("reload_time");
        protected static readonly int Shoot = Animator.StringToHash("shoot");

        protected float _time;

        public abstract void Something();
    }
}
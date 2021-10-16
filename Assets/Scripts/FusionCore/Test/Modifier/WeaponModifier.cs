using System;

namespace FusionCore.Test
{
    [Serializable]
    public class WeaponModifier
    {
        public string NameModifier;
        public WeaponModifierType ChangeParameter;
        public float AddValue;
    }
}
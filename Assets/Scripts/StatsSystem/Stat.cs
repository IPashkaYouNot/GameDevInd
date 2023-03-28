using StatsSystem.Enums;
using System;
using UnityEngine;

namespace StatsSystem
{
    [Serializable]
    public class Stat : ICloneable
    {
        [field: SerializeField] public StatType Type { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        public Stat(StatType type, float value)
        {
            Type = type;
            Value = value;
        }

        public void SetStatValue (float value) => Value = value;


        public static implicit operator float (Stat stat) => stat?.Value ?? 0;
        public object Clone() => new Stat(Type, Value);
    }
}

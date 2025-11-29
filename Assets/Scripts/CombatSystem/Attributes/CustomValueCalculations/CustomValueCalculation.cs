using UnityEngine;

namespace CombatSystem.Attributes.CustomValueCalculations
{
    public abstract class CustomValueCalculation : ScriptableObject
    {
        public abstract float Calculate(CombatSystemComponent source, CombatSystemComponent target);
    }
}

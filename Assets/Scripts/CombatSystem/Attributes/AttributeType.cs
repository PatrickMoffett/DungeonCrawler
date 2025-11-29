using UnityEngine;

namespace CombatSystem.Attributes
{
    [CreateAssetMenu(menuName = "CombatSystem/AttributeType")]
    public class AttributeType : ScriptableObject
    {
        // Default value for this attribute
        public float defaultValue;

        // Another attribute to use as a maximum value (optional). Example: MaxHealth
        public AttributeType maxAttribute;
    }
}
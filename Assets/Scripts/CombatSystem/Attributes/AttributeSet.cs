using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Attributes
{
    [Serializable]
    public class AttributeEntry
    {
        public AttributeType type;
        public Attribute value;
    }

    public class AttributeSet : MonoBehaviour
    {
        // This list shows up in the Inspector and will be populated with default attributes (by Reset function)
        [SerializeField] 
        private List<AttributeEntry> attributes = new List<AttributeEntry>();

        private Dictionary<AttributeType, Attribute> AttributeDictionary { get; set; } = new Dictionary<AttributeType, Attribute>();

        public void Reset()
        {
            // Clear any existing values so we always start fresh
            attributes.Clear();

            // Iterate through every AttributeType and create a new Attribute instance for each
            foreach (AttributeType type in Resources.LoadAll<AttributeType>(""))
            {
                attributes.Add(new AttributeEntry { type = type, value = new Attribute(type.defaultValue) });
            }       
        }
        private void Awake()
        {
            // Ensure the dictionary is populated before gameplay.
            InitializeAttributeDictionary();
            InitializeMaxAttributes();
        }

        /// <summary>
        /// Initializes the dictionary from the serialized list.
        /// </summary>
        public void InitializeAttributeDictionary()
        {
            AttributeDictionary = new Dictionary<AttributeType, Attribute>();
            foreach (var entry in attributes)
            {
                // This check prevents duplicate keys in case of user error.
                if (!AttributeDictionary.ContainsKey(entry.type))
                {
                    AttributeDictionary.Add(entry.type, entry.value);
                }
            }
        }

        private void InitializeMaxAttributes()
        {
            foreach (var entry in AttributeDictionary)
            {
                AttributeDictionary.TryGetValue(entry.Key, out var attribute);
                if (entry.Key.maxAttribute != null && attribute != null)
                {
                    AttributeDictionary.TryGetValue(entry.Key.maxAttribute, out var maxAttribute);
                    attribute.SetMaxAttribute(maxAttribute);
                }
            }
        }

        public void UpdateCurrentValues()
        {
            foreach (var pair in AttributeDictionary)
            {
                pair.Value.UpdateCurrentValue();
            }
        }

        public void ApplyModifier(AttributeModifier modifier)
        {
            if (AttributeDictionary.TryGetValue(modifier.attribute, out var attribute))
            {
                attribute.AddModifier(modifier);
            }
            else
            {
                Debug.LogWarning("Attribute not found: " + modifier.attribute);
            }
        }

        public void RemoveModifier(AttributeModifier modifier)
        {
            if (AttributeDictionary.TryGetValue(modifier.attribute, out var attribute))
            {
                attribute.RemoveModifier(modifier);
            }
            else
            {
                Debug.LogWarning("Attribute not found: " + modifier.attribute);
            }
        }

        public void ApplyInstantModifier(AttributeModifier modifier)
        {
            if (AttributeDictionary.TryGetValue(modifier.attribute, out var attribute))
            {
                attribute.InstantlyApply(modifier);
            }
            else
            {
                Debug.LogWarning("Attribute not found: " + modifier.attribute);
            }
        }
    
        public bool HasAttribute(AttributeType type)
        {
            return AttributeDictionary.ContainsKey(type);
        }

        public Attribute GetAttribute(AttributeType type)
        {
            if (AttributeDictionary.TryGetValue(type, out var attribute))
            {
                return attribute;
            }
            Debug.LogWarning("Attribute not found: " + type);
            return null;
        }
        public float GetCurrentAttributeValue(AttributeType type)
        {
            if (AttributeDictionary.TryGetValue(type, out var attribute))
            {
                return attribute.CurrentValue;
            }
            // ReSharper disable Unity.PerformanceCriticalCodeInvocation
            Debug.LogError("Attribute not found: " + type);
            // ReSharper restore Unity.PerformanceCriticalCodeInvocation
            return 0f;
        }

        public float GetBaseAttributeValue(AttributeType type)
        {
            if (AttributeDictionary.TryGetValue(type, out var attribute))
            {
                return attribute.BaseValue;
            }
            Debug.LogError("Attribute not found: " + type);
            return 0f;
        }

        // Exposing this for use by CombatSystemEditor, please refrain from other uses unless necessary
        // TODO: is there a way to just expose the dictionary to the editor without making it public?
        // like friend keyword in C++?
        public Dictionary<AttributeType, Attribute> GetAttributeDictionary()
        {
            return AttributeDictionary;
        }
    }
}
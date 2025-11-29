using System;
using CombatSystem.Attributes.CustomValueCalculations;
using UnityEngine;

namespace CombatSystem.Attributes
{
    [Serializable]
    public class AttributeModifierValue
    {
        public enum ValueType
        {
            Constant,
            AttributeBased,
            CustomCalculation
        }

        public enum AttributeSetToUse
        {
            Source,
            Target
        }
    
        public bool ValueTypeEqualsConstant()
        {
            return valueType == ValueType.Constant;
        }

        public bool ValueTypeEqualsAttributeBased()
        {
            return valueType == ValueType.AttributeBased;
        }

        public bool ValueTypeEqualsCustomCalc()
        {
            return valueType == ValueType.CustomCalculation;
        }
    
        [SerializeField]private ValueType valueType;
    
        [ShowIf(nameof(ValueTypeEqualsConstant))]
        [SerializeField]
        private float constantFloat;
    
        [ShowIf(nameof(ValueTypeEqualsAttributeBased))]
        [SerializeField]
        private AttributeSetToUse attributeSet;

        [ShowIf(nameof(ValueTypeEqualsAttributeBased))]
        [SerializeField]
        private AttributeType sourceAttributeType;

        [ShowIf( nameof(ValueTypeEqualsAttributeBased))]
        [SerializeField]
        private float preCoefficientAddition;

        [ShowIf( nameof(ValueTypeEqualsAttributeBased))]
        [SerializeField]
        private float coefficient;
    
        [ShowIf( nameof(ValueTypeEqualsAttributeBased))]
        [SerializeField]
        private float postCoefficientAddition;

        [ShowIf(nameof(ValueTypeEqualsCustomCalc))]
        [SerializeField]
        private CustomValueCalculation customValueCalculation;

        private float _value = 0.0f;

        public float GetValue()
        {
            return _value;
        }
        public float UpdateValue(CombatSystemComponent sourceCombatSystem, CombatSystemComponent targetCombatSystem)
        {
            
            {
                if (valueType == ValueType.AttributeBased
                    && attributeSet == AttributeSetToUse.Source
                    && !sourceCombatSystem)
                {
                    //TODO: Fix bug where source target has died and this prevents applying their effects
                    //try snapshotting their attribute set
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.LogError("No Source Combat System Found (May have been destroyed");
                    _value = 0f;
                    return _value;
                }
                switch (valueType)
                {
                    case ValueType.Constant:
                        _value =  constantFloat;
                        return _value;
                    case ValueType.AttributeBased:
                        if (attributeSet == AttributeSetToUse.Source)
                        {
                            _value = ((sourceCombatSystem.GetAttributeSet().GetCurrentAttributeValue(sourceAttributeType)
                                       + preCoefficientAddition) * coefficient) + postCoefficientAddition;
                            return _value;
                        }
                        else
                        {
                            if (!(targetCombatSystem))
                            {
                                Debug.LogError("Attribute Modifier Value missing Target Attribute Set");
                                _value = 0f;
                                return _value;
                            }
                            _value = ((targetCombatSystem.GetAttributeSet().GetCurrentAttributeValue(sourceAttributeType)
                                       + preCoefficientAddition) * coefficient) + postCoefficientAddition;
                            return _value;
                        }
                    case ValueType.CustomCalculation:
                        if (!customValueCalculation)
                        {
                            // ReSharper disable Unity.PerformanceCriticalCodeInvocation
                            Debug.LogError("Custom Calculation Not Set");
                            // ReSharper restore Unity.PerformanceCriticalCodeInvocation
                        }
                        _value = customValueCalculation.Calculate(sourceCombatSystem, targetCombatSystem);
                        return _value;
                    default:
                        Debug.LogError("Unsupported Value Type used");
                        _value = 0f;
                        return _value;
                }
            }
        }
    }
}

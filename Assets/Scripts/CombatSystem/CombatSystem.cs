using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Attributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace CombatSystem
{
    [RequireComponent(typeof(AttributeSet))]
    [RequireComponent(typeof(CombatTagContainer))]
    public class CombatSystemComponent : MonoBehaviour
    {
        [SerializeField] private List<StatusEffect> startingEffects = new List<StatusEffect>();

        private AttributeSet _attributeSet;
        private CombatTagContainer _combatTagContainer;

        private readonly List<AppliedStatusEffect> _currentStatusEffects = new List<AppliedStatusEffect>();

        public event Action<AppliedStatusEffect> OnStatusEffectAdded;
        public event Action<AppliedStatusEffect> OnStatusEffectRemoved;

        // Start is called before the first frame update
        private void Start()
        {
            _attributeSet = GetComponent<AttributeSet>();
            Assert.IsNotNull(_attributeSet, "AttributeSet is not assigned in CombatSystem.");

            _combatTagContainer = GetComponent<CombatTagContainer>();
            Assert.IsNotNull(_combatTagContainer, "CombatTagContainer is not assigned in CombatSystem.");
            //force update current values before apply effects
            _attributeSet.UpdateCurrentValues();

            //apply starting effects
            foreach (var effect in startingEffects)
            {
                OutgoingStatusEffect statusEffectToApply =
                    new OutgoingStatusEffect(this, effect);
                ApplyStatusEffect(statusEffectToApply);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public AppliedStatusEffect ApplyStatusEffect(OutgoingStatusEffect effectToApply)
        {
            //set this as the targetCombatSystem
            AppliedStatusEffect appliedStatusEffect = new AppliedStatusEffect(this, effectToApply);
            
            if (_combatTagContainer.HasAnyTag(appliedStatusEffect.StatusEffect.immunityTags)) // TODO: WTF
            {
                //if the target has any of the tags to ignore, don't apply the effect
                return null;
            }

            if (appliedStatusEffect.StatusEffect.durationType == StatusEffect.DurationType.Instant)
            {
                //apply all modifiers instantly
                foreach (var modifier in appliedStatusEffect.StatusEffect.attributeModifiers)
                {
                    _attributeSet.ApplyInstantModifier(modifier);
                }
            }
            else
            {
                _combatTagContainer.AddTags(appliedStatusEffect.StatusEffect.providedTags); // TODO: WTF

                //if the effect has a duration start a coroutine to remove the effect when it's done.
                if (appliedStatusEffect.StatusEffect.durationType == StatusEffect.DurationType.Duration)
                {
                    StartCoroutine(WaitToRemoveStatusEffect(appliedStatusEffect));
                }

                //Add it to the list of current status effects
                _currentStatusEffects.Add(appliedStatusEffect);

                //if the effect happens periodically, start a coroutine to do that
                if (appliedStatusEffect.StatusEffect.isPeriodic)
                {
                    StartCoroutine(ApplyPeriodicEffect(appliedStatusEffect));
                }
                else
                {
                    //otherwise add a modifier to each affected attribute
                    foreach (var modifier in appliedStatusEffect.StatusEffect.attributeModifiers)
                    {
                        _attributeSet.ApplyModifier(modifier);
                    }
                }
            }

            OnStatusEffectAdded?.Invoke(appliedStatusEffect);
            return appliedStatusEffect;
        }

        private IEnumerator ApplyPeriodicEffect(AppliedStatusEffect effectToApply)
        {
            //While we have this effect
            while (_currentStatusEffects.Contains(effectToApply))
            {
                //Apply Modifiers instantly
                foreach (var modifier in effectToApply.StatusEffect.attributeModifiers)
                {
                    _attributeSet.ApplyInstantModifier(modifier);
                }

                //and wait periodic rate before doing it again
                yield return new WaitForSeconds(effectToApply.StatusEffect.periodicRate.GetValue());
            }
        }

        public void RemoveStatusEffect(AppliedStatusEffect effectToRemove)
        {
            _currentStatusEffects.Remove(effectToRemove);
            _combatTagContainer.RemoveTags(effectToRemove.StatusEffect.providedTags); // TODO: WTF?
            if (effectToRemove.StatusEffect.durationType == StatusEffect.DurationType.Instant)
            {
                Debug.LogError("Tried to remove Instant Status Effect");
                return;
            }

            if (!effectToRemove.StatusEffect.isPeriodic)
            {
                foreach (var modifier in effectToRemove.StatusEffect.attributeModifiers)
                {
                    _attributeSet.RemoveModifier(modifier);
                }
            }

            OnStatusEffectRemoved?.Invoke(effectToRemove);
        }

        public List<AppliedStatusEffect> GetStatusEffects()
        {
            return _currentStatusEffects;
        }

        private IEnumerator WaitToRemoveStatusEffect(AppliedStatusEffect effectToRemove)
        {
            yield return new WaitForSeconds(effectToRemove.StatusEffect.duration.GetValue());
            RemoveStatusEffect(effectToRemove);
        }

        public AttributeSet GetAttributeSet()
        {
            return _attributeSet;
        }

        public bool CheckActivationCosts(StatusEffect activationCost)
        {
            //for every modifier
            foreach (var modifier in activationCost.attributeModifiers)
            {
#if UNITY_EDITOR
                //we're pretty much assuming subtract only at this point
                if (modifier.operation != AttributeModifier.Operator.Subtract)
                {
                    Debug.LogWarning("Using a modifier other than subtract as an Activation Cost.");
                }
#endif
                //if we subtract and the value is below 0
                if (modifier.operation == AttributeModifier.Operator.Subtract &&
                    _attributeSet.GetCurrentAttributeValue(modifier.attribute) < modifier.attributeModifierValue.GetValue())
                {
                    //don't apply the cost and return false
                    return false;
                }
            }

            return true;
        }

        public bool TryActivationCost(StatusEffect activationCost)
        {
            if (CheckActivationCosts(activationCost))
            {
                //apply the cost and return true
                OutgoingStatusEffect statusEffectToApply =
                    new OutgoingStatusEffect(this,activationCost);
                ApplyStatusEffect(statusEffectToApply);
                return true;
            }
            else
            {
                //if we can't apply the cost, return false
                return false;

            }
        }
    }
}

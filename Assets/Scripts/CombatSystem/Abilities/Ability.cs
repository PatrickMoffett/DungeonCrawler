using CombatSystem;
using CombatSystem.Attributes;
using UnityEditor;
using UnityEngine;

public class AbilityTargetData //Expand/inherit from this class as more data is needed
{
    public Vector3 sourceCharacterLocation;
    public Vector3 sourceCharacterDirection;
    public Vector3 targetLocation;
    public GameObject targetGameObject;
    public string animationTrigger;
}
public abstract class Ability : ScriptableObject
{
    public bool passiveAbility = false;

    protected GameObject _owner;
    protected AttributeSet _attributes;
    protected CombatSystemComponent _combatSystem;

    public StatusEffect activationCost;
    public StatusEffect cooldown;

    private AppliedStatusEffect _appliedCooldownEffect;

    public void Initialize(GameObject owner)
    {
        _owner = owner;
        _combatSystem = _owner.GetComponent<CombatSystemComponent>();
        _attributes = _owner.GetComponent<AttributeSet>();
    }
    public virtual bool CanActivate(AbilityTargetData activationData)
    {
        if (activationCost != null)
        {
            if (!_combatSystem.CheckActivationCosts(activationCost))
            {
                return false;
            }
        }
        if (_appliedCooldownEffect != null && _combatSystem.GetStatusEffects().Contains(_appliedCooldownEffect))
        {
            return false;
        }
        return true;
    }
    public virtual bool TryActivate(AbilityTargetData activationData)
    {
        if ((_appliedCooldownEffect == null || !_combatSystem.GetStatusEffects().Contains(_appliedCooldownEffect)) 
            &&
            (activationCost == null || _combatSystem.TryActivationCost(activationCost)))
        {
            Activate(activationData);
            
            if (cooldown != null)
            {
                OutgoingStatusEffect effect = new OutgoingStatusEffect(_combatSystem, cooldown);
                _appliedCooldownEffect = _combatSystem.ApplyStatusEffect(effect);
            }

            return true;
        }
        else
        {
            return false;
        }
    }
    protected abstract void Activate(AbilityTargetData activationData);
}

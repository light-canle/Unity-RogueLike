using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPotion : Consumable
{
    [SerializeField] private Effect.EffectType effectType;
    [SerializeField] private int effectTurns = 1;

    public Effect.EffectType EffectType { get => effectType; }
    public int EffectTurns { get => effectTurns; }

    public override bool Activate(Actor consumer)
    {
        // 효과를 부여함
        consumer.GetComponent<Fighter>().AddEffect(effectType, effectTurns);
        Consume(consumer);
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingPotion : Consumable
{
    [SerializeField] private Effect.EffectType effectType;
    [SerializeField] private int effectTurns = 6;
    [SerializeField] private int maximumRange = 5;

    public Effect.EffectType EffectType { get => effectType; }
    public int EffectTurns { get => effectTurns; }
    public int MaximumRange { get => maximumRange; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage("투척 포션을 던질 상대를 선택하세요.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.instance.AddMessage($"{consumer.name}(이)가 {target.name}에게 포션을 던졌다!", "#ffff00");
        target.GetComponent<Fighter>().AddEffect(effectType, effectTurns);
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}

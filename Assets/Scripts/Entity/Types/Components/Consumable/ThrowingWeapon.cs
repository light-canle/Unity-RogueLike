using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : Consumable
{
    [SerializeField] private int damage = 7;
    [SerializeField] private int maximumRange = 5;

    public int Damage { get => damage; }
    public int MaximumRange { get => maximumRange; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage("투척 무기를 던질 상대를 선택하세요.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        Action.RangeAction(consumer, target, damage);
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}

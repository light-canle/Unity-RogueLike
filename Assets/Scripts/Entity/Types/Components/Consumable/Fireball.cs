using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Consumable
{
    [SerializeField] private int damage = 12;
    [SerializeField] private int radius = 3;

    public int Damage { get => damage; }
    public int Radius { get => radius; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(true, radius);
        UIManager.instance.AddMessage($"파이어볼을 던질 곳을 선택하세요.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        foreach(Actor target in targets)
        {
            UIManager.instance.AddMessage($"{target.name}은(는) 파이어볼의 폭발에 휩싸였고, {damage}만큼의 피해를 입었다!", "#FF0000");
            target.GetComponent<Fighter>().Hp -= damage; // 이 파이어볼 공격은 AC를 무시한다.
        }

        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}

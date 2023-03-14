using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Consumable
{
    [SerializeField] private int damage = 20;
    [SerializeField] private int maximumRange = 5;

    public int Damage { get => damage; }
    public int MaximumRange { get => maximumRange; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage("번개 마법을 사용할 상대를 선택하세요.", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.instance.AddMessage($"번개가 큰 천둥 소리와 함께 {target.name}에게 내리 꽂혔고, {damage}의 피해를 입혔다!", "#ffffff");
        target.GetComponent<Fighter>().Hp -= damage; // 이 공격은 AC를 무시한다.
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}

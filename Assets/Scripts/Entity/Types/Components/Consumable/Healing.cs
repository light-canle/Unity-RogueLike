using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Consumable
{
    [SerializeField] private int amount = 0;
    public int Amount { get => amount; }

    public override bool Activate(Actor consumer)
    {
        int amountRecovered = consumer.GetComponent<Fighter>().Heal(amount);

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"당신은 {name}을(를) 사용해서 체력을 {amountRecovered}만큼 회복했다.", "#00ff00");
            Consume(consumer);
            return true;
        }
        else
        {
            UIManager.instance.AddMessage("당신의 체력은 이미 가득찼다.", "#808080");
            return false;
        }
    }
}

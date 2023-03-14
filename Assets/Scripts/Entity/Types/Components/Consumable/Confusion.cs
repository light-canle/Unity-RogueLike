using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confusion : Consumable
{
    [SerializeField] private int numberOfTurns = 10;

    public int NumberOfTurns { get => numberOfTurns; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage($"혼란 상태를 부여할 대상을 선택하세요.", "#63FFFF");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        if (target.TryGetComponent(out ConfusedEnemy confusedEnemy))
        {
            if (confusedEnemy.TurnsRemaining > 0)
            {
                UIManager.instance.AddMessage($"이미 혼란 상태인 적에게 중첩해서 쓸 수 없습니다.", "#ff0000");
                consumer.GetComponent<Inventory>().SelectedConsumable = null;
                consumer.GetComponent<Player>().ToggleTargetMode();
                return false;
            }     
        }
        else
        {
            confusedEnemy = target.gameObject.AddComponent<ConfusedEnemy>();
        }
        confusedEnemy.PreviousAI = target.AI;
        confusedEnemy.TurnsRemaining = NumberOfTurns;

        UIManager.instance.AddMessage($"{target.name}이 정신을 잃고 방황하기 시작합니다!", "#FF0000");
        target.AI = confusedEnemy;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen : Consumable
{
    [SerializeField] private int numberOfTurns = 10;

    public int NumberOfTurns { get => numberOfTurns; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage($"빙결 상태를 부여할 대상을 선택하세요.", "#63FFFF");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        if (target.TryGetComponent(out FrozenEnemy frozenEnemy))
        {
            if (frozenEnemy.TurnsRemaining > 0)
            {
                UIManager.instance.AddMessage($"이미 빙결 상태인 적에게 중첩해서 쓸 수 없습니다.", "#ff0000");
                consumer.GetComponent<Inventory>().SelectedConsumable = null;
                consumer.GetComponent<Player>().ToggleTargetMode();
                return false;
            }     
        }
        else if (target.TryGetComponent(out ConfusedEnemy confusedEnemy))
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
            frozenEnemy = target.gameObject.AddComponent<FrozenEnemy>();
        }
        frozenEnemy.PreviousAI = target.AI;
        frozenEnemy.TurnsRemaining = NumberOfTurns;

        UIManager.instance.AddMessage($"{target.name}의 몸이 차가워 지더니 순식간에 얼어붙어 버렸습니다!", "#00ffff");
        target.AI = frozenEnemy;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
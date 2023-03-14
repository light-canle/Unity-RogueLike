using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class HostileEnemy : AI
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private bool isFighting;

    private void OnValidate()
    {
        fighter = GetComponent<Fighter>();
        AStar = GetComponent<AStar>();
    }

    public override void RunAI()
    {
        if (!fighter.Target)
        {
            fighter.Target = GameManager.instance.Actors[0];
        } 
        else if (fighter.Target && !fighter.Target.IsAlive)
        {
            fighter.Target = null;
        }

        if (fighter.Target)
        {
            Vector3Int targetPosition = MapManager.instance.FloorMap.WorldToCell(fighter.Target.transform.position);
            if (isFighting || GetComponent<Actor>().FieldOfView.Contains(targetPosition))
            {
                if (!isFighting)
                {
                    isFighting = true;
                }

                float targetDistance = Vector3.Distance(transform.position, fighter.Target.transform.position);

                // 타겟과의 거리가 1.5칸 이내(대각선 1칸 또는 바로 옆칸)일 때 공격한다.
                // 이 부분을 변형하면 원거리 공격도 만들어 낼 수 있을 것이다.
                if (targetDistance <= 1.5f)
                {
                    Action.MeleeAction(GetComponent<Actor>(), fighter.Target);
                    return;
                }
                else
                {
                    // 자신의 시야안에 타겟이 보이지 않는 경우 타겟을 추적한다.
                    MoveAlongPath(targetPosition);
                    return;
                }
            }
        }

        Action.WaitAction();
    }

    public override AIState SaveState() => new AIState(
        type: "HostileEnemy"
    );
}

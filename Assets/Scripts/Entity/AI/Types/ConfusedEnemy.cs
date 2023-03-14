using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 혼란 상태에 빠진 적은 정해진 턴 수 동안 임의의 방향으로 이동하다가 원래의 AI로 돌아가게 된다.
/// 혼란 상태의 빠진 적이 임의로 설정한 방향에 대상이 있다면 공격할 것이다.
/// </summary>
[RequireComponent(typeof(Actor))]
public class ConfusedEnemy : AI
{
    [SerializeField] private AI previousAI;
    [SerializeField] private int turnsRemaining;

    public AI PreviousAI { get => previousAI; set => previousAI = value; }
    public int TurnsRemaining { get => turnsRemaining; set => turnsRemaining = value;}

    public override void RunAI()
    {
        // 혼란 효과의 턴 수가 끝난 경우 원래의 AI로 돌아가게 된다.
        if (turnsRemaining <= 0)
        {
            UIManager.instance.AddMessage($"{gameObject.name}(은)는 더 이상 혼란해 하지 않는다.", "#ff0000");
            GetComponent<Actor>().AI = previousAI;
            GetComponent<Actor>().AI.RunAI();
            Destroy(this);
        }
        else
        {
            // 임의의 방향으로 이동한다.
            Vector2Int direction = Random.Range(0, 8) switch
            {
                0 => new Vector2Int(0, 1),      // 북서
                1 => new Vector2Int(0, -1),     // 북
                2 => new Vector2Int(1, 0),      // 북동
                3 => new Vector2Int(-1, 0),     // 서
                4 => new Vector2Int(1, 1),      // 동
                5 => new Vector2Int(1, -1),     // 남서
                6 => new Vector2Int(-1, 1),     // 남
                7 => new Vector2Int(-1, -1),    // 남동
                _ => new Vector2Int(0, 0)
            };
            // 혼란 상태에 빠진 액터는 임의의 방향으로 이동하거나 공격을 하려고 시도할 것이다.
            // 만약 그것이 벽이라면 턴을 낭비하게 되는 것이다.
            Action.BumpAction(GetComponent<Actor>(), direction);
            turnsRemaining--;
        }
    }

    public override AIState SaveState() => new ConfusedState(
        type : "ConfusedEnemy",
        previousAI : previousAI,
        turnsRemaining : turnsRemaining
    );

    public void LoadState(ConfusedState state)
    {
        if (state.PreviousAI == "HostileEnemy")
        {
            previousAI = GetComponent<HostileEnemy>();
        }
        turnsRemaining = state.TurnsRemaining;
    }
}

[System.Serializable]
public class ConfusedState : AIState
{
    [SerializeField] private string previousAI;
    [SerializeField] private int turnsRemaining;
    public string PreviousAI { get => previousAI; set => previousAI = value; }
    public int TurnsRemaining { get => turnsRemaining; set => turnsRemaining = value; }

    public ConfusedState(string type = "", AI previousAI = null, int turnsRemaining = 0) : base(type)
    {
        this.previousAI = previousAI.GetType().ToString();
        this.turnsRemaining = turnsRemaining;
    }
}

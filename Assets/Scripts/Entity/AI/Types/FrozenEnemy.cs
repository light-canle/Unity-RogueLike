using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 혼란 상태에 빠진 적은 정해진 턴 수 동안 임의의 방향으로 이동하다가 원래의 AI로 돌아가게 된다.
/// 혼란 상태의 빠진 적이 임의로 설정한 방향에 대상이 있다면 공격할 것이다.
/// </summary>
[RequireComponent(typeof(Actor))]
public class FrozenEnemy : AI
{
    [SerializeField] private AI previousAI;
    [SerializeField] private int turnsRemaining;

    public AI PreviousAI { get => previousAI; set => previousAI = value; }
    public int TurnsRemaining { get => turnsRemaining; set => turnsRemaining = value;}

    public override void RunAI()
    {
        // 빙결 효과의 턴 수가 끝난 경우 원래의 AI로 돌아가게 된다.
        if (turnsRemaining <= 0)
        {
            UIManager.instance.AddMessage($"{gameObject.name}(은)는 더 이상 얼어붙어 있지 않다.", "#ff0000");
            GetComponent<Actor>().AI = previousAI;
            GetComponent<Actor>().AI.RunAI();
            Destroy(this);
        }
        else
        {
            // 빙결 상태에 빠진 액터는 아무것도 할 수 없다.
            turnsRemaining--;
        }
    }

    public override AIState SaveState() => new FrozenState(
        type : "FrozenEnemy",
        previousAI : previousAI,
        turnsRemaining : turnsRemaining
    );

    public void LoadState(FrozenState state)
    {
        if (state.PreviousAI == "HostileEnemy")
        {
            previousAI = GetComponent<HostileEnemy>();
        }
        turnsRemaining = state.TurnsRemaining;
    }
}

[System.Serializable]
public class FrozenState : AIState
{
    [SerializeField] private string previousAI;
    [SerializeField] private int turnsRemaining;
    public string PreviousAI { get => previousAI; set => previousAI = value; }
    public int TurnsRemaining { get => turnsRemaining; set => turnsRemaining = value; }

    public FrozenState(string type = "", AI previousAI = null, int turnsRemaining = 0) : base(type)
    {
        this.previousAI = previousAI.GetType().ToString();
        this.turnsRemaining = turnsRemaining;
    }
}

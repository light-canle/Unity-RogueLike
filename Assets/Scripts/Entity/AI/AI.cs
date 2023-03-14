using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class AI : MonoBehaviour
{
    [SerializeField] private AStar aStar;

    public AStar AStar { get => aStar; set => aStar = value; }

    private void OnValidate() => aStar = GetComponent<AStar>();

    public virtual void RunAI() { }

    /// <summary>
    /// targetPosition을 향해 이동한다.
    /// </summary>
    /// <param name="targetPosition">이동하려는 위치</param>
    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);
        Vector2 direction = aStar.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.MovementAction(GetComponent<Actor>(), direction);
    }

    public virtual AIState SaveState() => new AIState();
}

[System.Serializable]
public class AIState
{
    [SerializeField] private string type;
    public string Type { get => type; set => type = value; }

    public AIState(string type = "")
    {
        this.type = type;
    }
}

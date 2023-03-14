using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (+) 시야 범위를 조절하려면 fieldOfViewRange를 조절하면 된다.
public class Actor : Entity
{
    [SerializeField] private bool isAlive = true; // Read-Only
    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView = new List<Vector3Int>();
    [SerializeField] private AI ai;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipment;
    [SerializeField] private Fighter fighter;
    [SerializeField] private Level level;
    AdamMilVisibility algorithm;

    public int FieldOfViewRange { get => fieldOfViewRange; set => fieldOfViewRange = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> FieldOfView { get => fieldOfView; }
    public Inventory Inventory { get => inventory; }
    public Equipment Equipment { get => equipment; }
    public AI AI { get => ai; set => ai = value; }
    public Fighter Fighter { get => fighter; set => fighter = value; }
    public Level Level { get => level; set => level = value; }

    private void OnValidate()
    {
        if (GetComponent<Inventory>())
        {
            inventory = GetComponent<Inventory>();
        }
        if (GetComponent<AI>())
        {
            ai = GetComponent<AI>();
        }
        if (GetComponent<Fighter>())
        {
            fighter = GetComponent<Fighter>();
        }
        if (GetComponent<Level>())
        {
            level = GetComponent<Level>();
        }
        if (GetComponent<Equipment>())
        {
            equipment = GetComponent<Equipment>();
        }
    }
    private void Start()
    {
        AddToGameManager();
        
        if (isAlive)
        {
            algorithm = new AdamMilVisibility();
            UpdateFieldOfView();
        }
        else if (fighter != null)
        {
            fighter.Die();
        }
    }

    public override void AddToGameManager()
    {
        base.AddToGameManager();

        if (GetComponent<Player>())
        {
            GameManager.instance.InsertActor(this, 0);
        }
        else
        {
            GameManager.instance.AddActor(this);
        }
    }

    /// <summary>
    /// 플레이어의 현재 시야(보이는 영역)를 업데이트한다.
    /// </summary>
    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        // 보이는 영역을 계산
        algorithm.Compute(gridPosition, fieldOfViewRange, fieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }
    }

    public override EntityState SaveState() => new ActorState(
        name: name,
        blocksMovement: BlocksMovement,
        isAlive: IsAlive,
        isVisible: MapManager.instance.VisibleTiles.Contains(MapManager.instance.FloorMap.WorldToCell(transform.position)),
        position : transform.position,
        currentAI : ai != null ? AI.SaveState() : null,
        fighterState : fighter != null ? fighter.SaveState() : null,
        levelState: level != null && GetComponent<Player>() ? level.SaveState() : null
    );

    public void LoadState(ActorState state)
    {
        transform.position = state.Position;
        isAlive = state.IsAlive;

        if (!IsAlive)
        {
            GameManager.instance.RemoveActor(this);
        }

        if (!state.IsVisible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        if (state.CurrentAI != null)
        {
            if (state.CurrentAI.Type == "HostileEnemy")
            {
                ai = GetComponent<HostileEnemy>();
            }
            else if (state.CurrentAI.Type == "ConfusedEnemy")
            {
                ai = gameObject.AddComponent<ConfusedEnemy>();

                ConfusedState confusedState = state.CurrentAI as ConfusedState;
                ((ConfusedEnemy)ai).LoadState(confusedState);
            }
        }

        if (state.FighterState != null)
        {
            fighter.LoadState(state.FighterState);
        }

        if (state.LevelState != null)
        {
            level.LoadState(state.LevelState);
        }
    }
}

[System.Serializable]
public class ActorState : EntityState
{
    [SerializeField] private bool isAlive;
    [SerializeField] private AIState currentAI;
    [SerializeField] private FighterState fighterState;
    [SerializeField] private LevelState levelState;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public AIState CurrentAI { get => currentAI; set => currentAI = value; }
    public FighterState FighterState { get => fighterState; set => fighterState = value; }
    public LevelState LevelState { get => levelState; set => levelState = value; }

    public ActorState(EntityType type = EntityType.Actor, string name = "", bool blocksMovement = false, bool isVisible = false,
        Vector3 position = new Vector3(), bool isAlive = true, AIState currentAI = null, FighterState fighterState = null,
        LevelState levelState = null)
        : base(type, name, blocksMovement, isVisible, position)
    {
        this.isAlive = isAlive;
        this.currentAI = currentAI;
        this.fighterState = fighterState;
        this.levelState = levelState;
    }
}

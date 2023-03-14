// Source : https://github.com/Chizaruu/Unity-RL-Tutorial
// Youtube Channel : https://www.youtube.com/channel/UC8__XEn9chu9LYDxFC4WzIA
// Thank you for your hard work moving the original Python tutorial to Unity!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // 전체 맵을 볼 지 플레이어 주변만 보일지를 결정하는 변수
    // 단 여기서는 변수만 있고 실제 설정과 키 입력은 Player.cs의 OnChangeCamera()에서 한다.
    private bool cameraFullMapView = true;

    [Header("Time")]
    [SerializeField] private float baseTime = 0.075f;
    [SerializeField] private float delayTime; // Read-Only


    [Header("Entities")]
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities;
    [SerializeField] private List<Actor> actors;

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;
    public bool CameraFullMapView { get => cameraFullMapView; set => cameraFullMapView = value; }
    public bool IsPlayerTurn { get => isPlayerTurn; }
    public List<Entity> Entities { get => entities; }
    public List<Actor> Actors { get => actors; }
    public Sprite DeadSprite { get => deadSprite; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

        if (sceneState is not null)
        {
            LoadState(sceneState.GameState, true);
        }
        else
        {
            entities = new List<Entity>();
            actors = new List<Actor>();
        }
    }

    /*private void Start()
    {
        // 플레이어 프리팹을 가져옴
        Instantiate(Resources.Load<GameObject>("Player")).name = "Player";
    }*/
    private void StartTurn()
    {
        //Debug.Log($"{entities[entityNum].name} starts its turn!");
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = true;
        }
        else
        {
            if (actors[actorNum].AI != null)
            {
                actors[actorNum].AI.RunAI();
            }
            else
            {
                Action.WaitAction();
            }
        } 

    }
    // 액터의 턴을 끝내는 함수
    public void EndTurn()
    {
        // 자신의 턴이 끝날 때 상태효과 점검
        actors[actorNum].GetComponent<Fighter>().EffectCheck();
        //Debug.Log($"{entities[entityNum].name} ends its turn!");
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = false;
            // 효과 리스트 UI 표시
            UIManager.instance.SetPlayerEffectListText(actors[actorNum].Fighter.Effects);
        }

        if (actorNum == actors.Count - 1)
        {
            actorNum = 0;
        }
        else
        {
            actorNum++;
        }
        StartCoroutine(TurnDelay());
    }
    // 자신의 차례가 올 때까지 기다림
    private IEnumerator TurnDelay()
    {
        yield return new WaitForSeconds(delayTime);
        StartTurn();
    }

    public void AddEntity(Entity entity)
    {
        if (entity is null) return;
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Add(entity);
    }

    public void InsertEntity(Entity entity, int index)
    {
        if (entity is null) return;
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Insert(index, entity);
    }
    public void RemoveEntity(Entity entity)
    {
        entity.gameObject.SetActive(false);
        entities.Remove(entity);
    }

    public void AddActor(Actor actor)
    {
        if (actor is null) return;
        actors.Add(actor);
        delayTime = SetTime();
    }
    public void InsertActor(Actor actor, int index)
    {
        if (actor is null) return;
        actors.Insert(index, actor);
        delayTime = SetTime();
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
        delayTime = SetTime();
    }

    public void RefreshPlayer()
    {
        actors[0].UpdateFieldOfView();
        actors[0].GetComponent<Player>().ChangeCamera();
        actors[0].GetComponent<Player>().ChangeCamera();
    }

    /// <summary>
    /// 해당 위치에 있는 엔티티가 움직임을 막는 엔티티인지 판단한다.
    /// </summary>
    /// <param name="location">입력받는 위치</param>
    /// <returns>해당 위치에 엔티티가 있고, 그 엔티티의 BlocksMovement가 true이면 그 entity를 반환한다.</returns>
    public Actor GetActorAtLocation(Vector3 location)
    {
        foreach(Actor actor in actors)
        {
            if (actor.BlocksMovement && actor.transform.position == location)
            {
                return actor;
            }
        }

        return null;
    }

    private float SetTime() => baseTime / actors.Count;

    public GameState SaveState()
    {
        // actors[0]은 플레이어를 뜻함
        foreach (Item item in actors[0].Inventory.Items)
        {

            AddEntity(item);
        }

        GameState gameState = new GameState(entities: entities.ConvertAll(x => x.SaveState()));

        foreach (Item item in actors[0].Inventory.Items)
        {
            RemoveEntity(item);
        }

        return gameState;
    }

    public void LoadState(GameState state, bool canRemovePlayer)
    {
        isPlayerTurn = false; // 플레이어가 맵이 로딩되는 도중 움직이는 것을 막는다.

        Reset(canRemovePlayer);
        StartCoroutine(LoadEntityStates(state.Entities, canRemovePlayer));
    }

    private IEnumerator LoadEntityStates(List<EntityState> entityStates, bool canPlacePlayer)
    {
        int entityState = 0;
        while(entityState < entityStates.Count)
        {
            yield return new WaitForEndOfFrame();

            if (entityStates[entityState].Type == EntityState.EntityType.Actor)
            {
                ActorState actorState = entityStates[entityState] as ActorState;

                string entityName = entityStates[entityState].Name.Contains("의 시체") ?
                entityStates[entityState].Name.Substring(0, entityStates[entityState].Name.LastIndexOf("의")) : entityStates[entityState].Name;

                if (entityName == "플레이어" && !canPlacePlayer)
                {
                    actors[0].transform.position = entityStates[entityState].Position;
                    RefreshPlayer();
                    entityState++;
                    continue;
                }

                Actor actor = MapManager.instance.CreateEntity(entityName, actorState.Position).GetComponent<Actor>();

                actor.LoadState(actorState);
                
            }
            else if (entityStates[entityState].Type == EntityState.EntityType.Item)
            {
                ItemState itemState = entityStates[entityState] as ItemState;
                string entityName = entityStates[entityState].Name.Contains("(E)") ?
                    entityStates[entityState].Name.Replace(" (E)", "") : entityStates[entityState].Name;

                if (itemState.Parent == "플레이어" && !canPlacePlayer)
                {
                    entityState++;
                    continue;
                }

                Item item = MapManager.instance.CreateEntity(entityName, itemState.Position).GetComponent<Item>();

                item.LoadState(itemState);
            }

            entityState++;
        }
        isPlayerTurn = true; // 로드된 후에는 플레이어의 이동을 허락함
    }

    public void Reset(bool canRemovePlayer = false)
    {
        if (entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (!canRemovePlayer && entity.GetComponent<Player>())
                {
                    continue;
                }

                Destroy(entity.gameObject);
            }

            if (canRemovePlayer) { 
                entities.Clear();
                actors.Clear();
            }
            else
            {
                entities.RemoveRange(1, entities.Count - 1);
                actors.RemoveRange(1, actors.Count - 1);
            }
        }
    }
}

[System.Serializable]
public class GameState
{
    [SerializeField] private List<EntityState> entities;

    public List<EntityState> Entities { get => entities; set => entities = value; }

    public GameState(List<EntityState> entities)
    {
        this.entities = entities;
    }
}
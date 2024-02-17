using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;

// (+) 만약 마법 지도(맵을 다 드러냄)의 주문서를 구현하려면 모든 타일을 visibleTiles에 넣으면 된다.

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [Header("Map Settings")]
    // 맵 최대 크기
    [SerializeField] private int width = 80; 
    [SerializeField] private int height = 45;
    // 방의 최대, 최소 크기 / 한 단계당 방 최대 개수
    [SerializeField] private int roomMaxSize = 12;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;
    [SerializeField] private TileBase upStairsTile;
    [SerializeField] private TileBase downStairsTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<Vector3Int> visibleTiles;
    [SerializeField] private List<RectangularRoom> rooms;
    private Dictionary<Vector3Int, TileData> tiles;
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    public int Width { get => width; }
    public int Height { get => height; }
    public TileBase FloorTile { get => floorTile; }
    public TileBase WallTile { get => wallTile; }
    public TileBase UpStairsTile { get => upStairsTile; }
    public TileBase DownStairsTile { get => downStairsTile; }
    public Tilemap FloorMap { get => floorMap; }
    public Tilemap ObstacleMap { get => obstacleMap; }
    public Tilemap FogMap { get => fogMap; }

    public List<RectangularRoom> Rooms { get => rooms; }
    public List<Vector3Int> VisibleTiles { get => visibleTiles; }
    public Dictionary<Vector2Int, Node> Nodes { get => nodes; set => nodes = value; }

    private void Awake()
    {
        Debug.Log("!");
        Debug.Log(instance);
        Debug.Log(this);
        if (instance == null) { 
            instance = this;
        }
        else { 
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Dungeon")
            return;

        SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

        if (sceneState is not null)
        {
            LoadState(sceneState.MapState);
        }
        else
        {
            GenerateDungeon(true);
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        //Instantiate(Resources.Load<GameObject>("NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";
        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;
    }

    public void GenerateDungeon(bool isNewGame = false, bool isLastFloor = false)
    {
        //Debug.Log(floorMap.cellBounds.size.x);
        if (floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }
        else
        {
            rooms = new List<RectangularRoom>();
            tiles = new Dictionary<Vector3Int, TileData>();
            visibleTiles = new List<Vector3Int>();
        }
       

        Procgen procGen = new Procgen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, rooms, isNewGame, isLastFloor);
        //Vector3Int wallPosition = new Vector3Int(wallBounds.min.x + x, wallBounds.min.y + y, 0);
        //obstacleMap.SetTile(wallPosition, wallTile);
        AddTileMapToDictionary(floorMap);
        AddTileMapToDictionary(obstacleMap);
        SetupFogMap();

        if (!isNewGame)
        {
            GameManager.instance.RefreshPlayer();
        }
    }

    ///<summary>입력한 좌표가 해당 맵의 영역에 들어와 있으면 True를 반환한다.</summary>
    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    /// <summary>
    /// 해당 이름에 대응하는 엔티티를 position 위치에 생성한다.
    /// </summary>
    /// <param name="entity">소환하려는 엔티티의 이름 - 대문자로 시작하는 영어 이름</param>
    /// <param name="position">소환하려는 위치</param>
    public GameObject CreateEntity(string entity, Vector2 position)
    {
        GameObject entityObject = Instantiate(Resources.Load<GameObject>($"{entity}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        entityObject.name = entity;
        return entityObject;
    }

    public string ConvertKorean(string englishName)
    {
        switch (englishName)
        {
            case "Potion of Health": return "치유의 포션";
            case "Potion of Regeneration": return "재생의 포션";
            case "Potion of Rage": return "분노의 포션";
            case "Potion of Protection": return "방어의 포션";
            case "Potion of Magical Vision": return "마법 시야의 포션";

            case "Fireball Scroll": return "화염구의 주문서";
            case "Lightning Scroll": return "번개의 주문서";
            case "Confusion Scroll": return "혼란의 주문서";

            case "Player": return "플레이어";
            case "Rat": return "쥐";
            case "Slime": return "슬라임";
            default: return "??";
        }
    }

    /// <summary>
    /// 안개의 상태를 플레이어의 시야에 따라 변경한다. 
    /// </summary>
    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (Vector3Int pos in visibleTiles)
        {
            // 탐험하지 않은 타일을 드러냄
            if (!tiles[pos].IsExplored)
            {
                tiles[pos].IsExplored = true;
            }
            // 드러냈으나 보이지 않는 타일로 바꿈
            tiles[pos].IsVisible = false;
            fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }
        // 플레이어 시야 내에 있는 타일들을 다시 그림
        visibleTiles.Clear();

        foreach(Vector3Int pos in playerFOV)
        {
            tiles[pos].IsVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    /// <summary>
    /// 플레이어의 시야에 들어왔는지에 여부에 따라 Entity가 그려질 지 아닐지를 판단한다.
    /// (+) 시야에 관계없이 모든 Entity의 위치가 보이게 하려면 이 부분을 조정하면 된다.
    /// </summary>
    public void SetEntitiesVisibilities()
    {
        foreach (Entity entity in GameManager.instance.Entities)
        {
            if (entity.GetComponent<Player>())
            {
                continue;
            }

            Vector3Int entityPosition = floorMap.WorldToCell(entity.transform.position);

            if (visibleTiles.Contains(entityPosition))
            {
                entity.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                entity.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    /// <summary>
    /// MapManager의 tiles Dictionary에 인자로 전달한 tilemap을 넣는다.
    /// </summary>
    /// <param name="tilemap">tiles에 넣을 Tilemap</param>
    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }

            TileData tile = new TileData(
                name: tilemap.GetTile(pos).name,
                isExplored: false,
                isVisible: false
            );
            tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach(Vector3Int pos in tiles.Keys)
        {
            if (!fogMap.HasTile(pos))
            {
                fogMap.SetTile(pos, fogTile);
                fogMap.SetTileFlags(pos, TileFlags.None);
            }

            if (tiles[pos].IsExplored)
            {
                fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }
            else
            {
                fogMap.SetColor(pos, Color.white);
            }
        }
    }

    /// <summary>
    /// 입력받은 위치가 이동가능한 위치(맵의 영역 안이고, 벽이 아님)인지를 검사한다.
    /// </summary>
    /// <param name="futurePosition">이동할 위치</param>
    /// <returns>이동가능한 경우 true를 반환한다.</returns>
    public bool IsValidPosition(Vector3 futurePosition)
    {
        Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);
        if (!InBounds(gridPosition.x, gridPosition.y) ||
            obstacleMap.HasTile(gridPosition))
            return false;

        return true;
    }

    private void Reset()
    {
        rooms.Clear();
        tiles.Clear();
        visibleTiles.Clear();
        nodes.Clear();

        floorMap.ClearAllTiles();
        obstacleMap.ClearAllTiles();
        fogMap.ClearAllTiles();
    }

    public MapState SaveState() => new MapState(tiles, rooms);

    public void LoadState(MapState mapState)
    {
        if (floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }

        rooms = mapState.StoredRooms;
        tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int(
            (int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);
        if (visibleTiles.Count > 0)
        {
            visibleTiles.Clear();
        }
        
        foreach (Vector3Int pos in tiles.Keys)
        {
            if (tiles[pos].Name == floorTile.name)
            {
                floorMap.SetTile(pos, floorTile);
            }
            else if (tiles[pos].Name == wallTile.name)
            {
                obstacleMap.SetTile(pos, wallTile);
            }
            else if (tiles[pos].Name == upStairsTile.name)
            {
                floorMap.SetTile(pos, upStairsTile);
            }
            else if (tiles[pos].Name == downStairsTile.name)
            {
                floorMap.SetTile(pos, downStairsTile);
            }
        }
        SetupFogMap();
    }
}

[System.Serializable]
public class MapState
{
    [SerializeField] private Dictionary<Vector3, TileData> storedTiles;
    [SerializeField] private List<RectangularRoom> storedRooms;
    public Dictionary<Vector3, TileData> StoredTiles { get => storedTiles; set => storedTiles = value; }
    public List<RectangularRoom> StoredRooms { get => storedRooms; set => storedRooms = value; }

    public MapState(Dictionary<Vector3Int, TileData> tiles, List<RectangularRoom> rooms)
    {
        storedTiles = tiles.ToDictionary(x => (Vector3)x.Key, x => x.Value);
        storedRooms = rooms;
    }
}

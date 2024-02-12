using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysRadnom = System.Random;
using UnityRandom = UnityEngine.Random;

sealed class Procgen : MonoBehaviour
{
    private List<Tuple<int, int>> maxItemsByFloor = new List<Tuple<int, int>> { 
        new Tuple<int, int>(1, 1),
        new Tuple<int, int>(6, 2),
        new Tuple<int, int>(16, 3),
    };

    private List<Tuple<int, int>> maxMonstersByFloor = new List<Tuple<int, int>> {
        new Tuple<int, int>(1, 2),
        new Tuple<int, int>(6, 3),
        new Tuple<int, int>(11, 4),
        new Tuple<int, int>(16, 5),
    };

    private List<Tuple<int, string, int>> itemChances = new List<Tuple<int, string, int>>
    {
        // 포션
        new Tuple<int, string, int>(0, "치유의 포션", 35),
        new Tuple<int, string, int>(3, "마법 시야의 포션", 10),
        new Tuple<int, string, int>(5, "방어의 포션", 15),
        new Tuple<int, string, int>(5, "분노의 포션", 15),
        new Tuple<int, string, int>(6, "재생의 포션", 7),
        // 1 ~ 5층 무기, 방어구
        new Tuple<int, string, int>(4, "검", 5), new Tuple<int, string, int>(4, "가죽 갑옷", 15),
        // 6 ~ 10층 무기, 방어구
        new Tuple<int, string, int>(9, "레이피어", 4), new Tuple<int, string, int>(9, "사슬 갑옷", 10), new Tuple<int, string, int>(9, "신속의 갑옷", 10),
        new Tuple<int, string, int>(9, "장검", 4), new Tuple<int, string, int>(9, "메이스", 4),
        // 11 ~ 17층 무기, 방어구
        new Tuple<int, string, int>(14, "룬소드", 4), new Tuple<int, string, int>(14, "미늘 갑옷", 8), new Tuple<int, string, int>(14, "미스릴 갑옷", 8),
        new Tuple<int, string, int>(14, "전투도끼", 4), new Tuple<int, string, int>(14, "시미터", 4),
        // 18 ~ 층 무기, 방어구
        new Tuple<int, string, int>(18, "할버드", 2), new Tuple<int, string, int>(18, "판금 갑옷", 4), new Tuple<int, string, int>(18, "미스릴 판금 갑옷", 4),
        new Tuple<int, string, int>(18, "초대형 낫", 2),
        // 반지
        new Tuple<int, string, int>(6, "회피의 반지", 3), new Tuple<int, string, int>(6, "집중의 반지", 3),
        // 주문서 
        new Tuple<int, string, int>(2, "강화의 주문서", 1),
        new Tuple<int, string, int>(5, "혼란의 주문서", 10),
        new Tuple<int, string, int>(6, "번개의 주문서", 25), 
        new Tuple<int, string, int>(12, "화염구의 주문서", 25), 
    };
    // (층 수, 몹 이름, 가중치)
    // 같은 몹 이름이 여러 개 있는 경우 해당 층에서 가장 높은 층 수를 가진 데이터만을 취한다.
    private List<Tuple<int, string, int>> monsterChances = new List<Tuple<int, string, int>>
    {
        // 1 ~ 5층 적
        new Tuple<int, string, int>(1, "쥐", 60), new Tuple<int, string, int>(3, "개미", 20),
        new Tuple<int, string, int>(3, "박쥐", 20), new Tuple<int, string, int>(3, "도마뱀", 15),
        new Tuple<int, string, int>(5, "박쥐", 40), new Tuple<int, string, int>(5, "도마뱀", 30),
        // 6 ~ 10층 적
        new Tuple<int, string, int>(6, "쥐", 20),
        new Tuple<int, string, int>(6, "고블린", 40),
        new Tuple<int, string, int>(7, "쥐", 0),
        new Tuple<int, string, int>(7, "박쥐", 10), new Tuple<int, string, int>(7, "도마뱀", 10),
        new Tuple<int, string, int>(7, "고블린", 60), new Tuple<int, string, int>(7, "슬라임", 15),
        new Tuple<int, string, int>(8, "독사", 10), new Tuple<int, string, int>(8, "스켈레톤 전사", 10),
        new Tuple<int, string, int>(8, "벌", 10), 
        new Tuple<int, string, int>(9, "박쥐", 0), new Tuple<int, string, int>(9, "도마뱀", 0),
        new Tuple<int, string, int>(9, "개미", 0),
        new Tuple<int, string, int>(10, "홉고블린", 25),
        // 11 ~ 17층 적
        new Tuple<int, string, int>(11, "고블린", 40), new Tuple<int, string, int>(11, "오우거", 10),
        new Tuple<int, string, int>(12, "고블린", 20), new Tuple<int, string, int>(12, "오우거", 30),
        new Tuple<int, string, int>(12, "화염 박쥐", 30),
        new Tuple<int, string, int>(13, "오우거 전사", 10), new Tuple<int, string, int>(13, "오우거", 60),
        new Tuple<int, string, int>(14, "슬라임", 0), new Tuple<int, string, int>(14, "오우거 마법사", 10),
        new Tuple<int, string, int>(15, "화염 슬라임", 10),
        // 18 ~ 21층 적
        new Tuple<int, string, int>(18, "돌 골렘", 10), new Tuple<int, string, int>(18, "크리스탈 골렘", 10),
    };

    public int GetMaxValueForFloor(List<Tuple<int, int>> values, int floor)
    {
        int currentValue = 0;

        foreach (Tuple<int, int> value in values)
        {
            if (floor >= value.Item1)
            {
                currentValue = value.Item2;
            }
        }

        return currentValue;
    }

    public List<string> GetEntitiesAtRandom(List<Tuple<int, string, int>> chances, int numberOfEntities, int floor)
    {
        List<string> entities = new List<string>();
        List<int> weightedChances = new List<int>();

        foreach(Tuple<int, string, int> chance in chances)
        {
            if (floor >= chance.Item1)
            {
                // 이미 존재하는 경우 나중 것의 weightedChance로 교체
                if (entities.Contains(chance.Item2))
                {
                    weightedChances[entities.IndexOf(chance.Item2)] = chance.Item3;
                }
                else
                {
                    entities.Add(chance.Item2);
                    weightedChances.Add(chance.Item3);
                }
            }
        }

        SysRadnom rnd = new SysRadnom();
        List<string> chosenEntities = rnd.Choices(entities, weightedChances, numberOfEntities);

        return chosenEntities;
    }

    /// <summary>
    /// 던전 맵을 생성한다.
    /// </summary>
    /// <param name="mapWidth">전체 맵의 너비</param>
    /// <param name="mapHeight">전체 맵의 높이</param>
    /// <param name="roomMaxSize">방의 최대 크기</param>
    /// <param name="roomMinSize">방의 최소 크기</param>
    /// <param name="maxRooms">방의 최대 개수</param>
    /// <param name="rooms">방들을 담은 리스트</param>
    /// <param name="isNewGame">새로운 게임인지 여부</param>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms, bool isNewGame)
    {
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            // 방의 크기와 위치 설정
            int roomWidth = UnityRandom.Range(roomMinSize, roomMaxSize);
            int roomHeight = UnityRandom.Range(roomMinSize, roomMaxSize);

            int roomX = UnityRandom.Range(0, mapWidth - roomWidth - 1);
            int roomY = UnityRandom.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            // 만들어진 방이 앞에서 생성된 방들과 겹치는지 확인한다.
            if (newRoom.Overlaps(rooms))
            {
                continue;
            }
            // 겹치지 않는다면 이 방은 유효한 것
            // 이 방의 영역에 따라 빈 공간(돌아다닐 수 있는 통로)을 만들고 벽으로 두른다.
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        // 바닥 타일이 깔려 있는 경우 벽을 설치하지 않음
                        if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }

                    else
                    {
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                // 이 방과 전 방 사이를 통로로 연결한다.
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
                // 첫 방에는 엔티티를 놓지 않는다.
                PlaceEntities(newRoom, SaveManager.instance.CurrentFloor);
            }

            
            // 방을 리스트에 추가
            rooms.Add(newRoom);
        }
        // 마지막 방에 계단을 위치시킨다.
        MapManager.instance.FloorMap.SetTile((Vector3Int)rooms[rooms.Count - 1].RandomPoint(), MapManager.instance.DownStairsTile);

        // 첫번째 방에 플레이어와 위로 가는 계단을 위치시킨다.
        Vector3Int playerPos = (Vector3Int)rooms[0].RandomPoint();

        while (GameManager.instance.GetActorAtLocation(playerPos) is not null)
        {
            playerPos = (Vector3Int)rooms[0].RandomPoint();
        }

        MapManager.instance.FloorMap.SetTile(playerPos, MapManager.instance.UpStairsTile);

        if (!isNewGame)
        {
            GameManager.instance.Actors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);
        }
        else
        { 
            GameObject player = MapManager.instance.CreateEntity("플레이어", (Vector2Int)playerPos);
            Actor playerActor = player.GetComponent<Actor>();

            Item starterWeapon = MapManager.instance.CreateEntity("단검", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterArmor = MapManager.instance.CreateEntity("천 갑옷", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterItem = MapManager.instance.CreateEntity("치유의 포션", (Vector2Int)playerPos).GetComponent<Item>();

            playerActor.Inventory.Add(starterWeapon);
            playerActor.Inventory.Add(starterArmor);
            playerActor.Inventory.Add(starterItem);

            playerActor.Equipment.EquipToSlot("Weapon", starterWeapon, false);
            playerActor.Equipment.EquipToSlot("Armor", starterArmor, false);
        }
    }

    /// <summary>
    /// oldRoom과 newRoom사이를 잇는다.
    /// </summary>
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (UnityRandom.value < 0.5f)
        {
            // 수평으로 움직인 후 수직으로 움직인다.
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // 수직으로 움직인 후 수평으로 움직인다.
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // 통로를 생성한다. 
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // 통로를 타일로 설치한다.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            // 통로를 둘러싸는 벽 설치
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        // 바닥 타일인 경우 
        if (MapManager.instance.FloorMap.GetTile(pos)){
            return true;
        }
        // 아닌 경우
        else
        {
            MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.WallTile);
            return false;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }
        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }

    /// <summary>
    /// 방에다 몬스터를 배치한다.
    /// </summary>
    /// <param name="newRoom">몬스터를 배치할 방</param>
    /// <param name="floor">층 수</param>
    private void PlaceEntities(RectangularRoom newRoom, int floorNumber)
    {
        // 방에 생성할 몬스터, 아이템의 수를 설정
        int numberOfMonsters = UnityRandom.Range(0, GetMaxValueForFloor(maxMonstersByFloor, floorNumber) + 1);
        int numberOfItems = UnityRandom.Range(0, GetMaxValueForFloor(maxItemsByFloor, floorNumber) + 1);

        List<string> monsterNames = GetEntitiesAtRandom(monsterChances, numberOfMonsters, floorNumber);
        List<string> itemNames = GetEntitiesAtRandom(itemChances, numberOfItems, floorNumber);

        List<string> entityNames = monsterNames.Concat(itemNames).ToList();

        foreach (string entityName in entityNames)
        {
            Vector3Int entityPos = (Vector3Int)newRoom.RandomPoint();

            while (GameManager.instance.GetActorAtLocation(entityPos) is not null)
            {
                entityPos = (Vector3Int)newRoom.RandomPoint();
            }

            MapManager.instance.CreateEntity(entityName, (Vector2Int)entityPos);
        }
    }
}

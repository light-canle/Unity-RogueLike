using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectangularRoom
{
    [SerializeField] private int x, y, width, height;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    /// <summary>
    /// 새로운 사각형 모양의 방을 생성한다.
    /// </summary>
    /// <param name="x">방의 왼쪽 위 모서리 위치의 x좌표</param>
    /// <param name="y">방의 왼쪽 위 모서리 위치의 y좌표</param>
    /// <param name="width">방의 너비</param>
    /// <param name="height">방의 높이</param>
    public RectangularRoom(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// 방의 중심을 구하는 메소드
    /// </summary>
    /// <returns>방의 중심의 좌표를 반환한다.</returns>
    public Vector2Int Center() => new Vector2Int(x + width / 2, y + height / 2);

    /// <summary>
    /// 방 안의 임의의 지점을 반환한다.
    /// </summary>
    /// <returns>임의의 지점의 좌표를 반환함</returns>
    public Vector2Int RandomPoint() => new Vector2Int(Random.Range(x + 1, x + width - 1), Random.Range(y + 1, y + height - 1));

    /// <summary>
    /// 방의 영역을 Bounds형으로 반환한다.
    /// </summary>
    public Bounds GetBounds() => new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));
    
    /// <summary>
    /// 방의 영역을 BoundsInt형으로 반환한다.
    /// </summary>
    public BoundsInt GetBoundsInt() => new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));

    /// <summary>
    /// 현재 방이 리스트에 있는 다른 방들과 겹치는지 검사한다.
    /// </summary>
    /// <param name="otherRooms">다른 방들이 들어있는 리스트</param>
    /// <returns>리스트 내 다른 방과 단 1개라도 겹치는 경우 true를 반환한다.</returns>
    public bool Overlaps(List<RectangularRoom> otherRooms)
    {
        foreach(RectangularRoom otherRoom in otherRooms)
        {
            if (GetBounds().Intersects(otherRoom.GetBounds()))
            {
                return true;
            }
        }
        return false;
    }
    
}

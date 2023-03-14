using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 0;
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();

    public int Capacity { get => capacity; }
    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> Items { get => items; }

    public void Add(Item item)
    {
        items.Add(item);
        item.transform.SetParent(transform);
        GameManager.instance.RemoveEntity(item);
    }
    /// <summary>
    /// 해당 아이템을 인벤토리에서 제거하고 바닥에 떨어뜨린다.
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    public void Drop(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
        item.GetComponent<SpriteRenderer>().enabled = true;
        item.AddToGameManager();
        UIManager.instance.AddMessage($"당신은 {item.name}을(를) 떨어뜨렸다.", "#ff0000");
    }
}

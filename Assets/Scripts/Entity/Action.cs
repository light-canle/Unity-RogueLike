using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public static class Action
{
    /// <summary>
    /// 아이템을 줍는 행동
    /// 아이템을 주워서 인벤토리에 넣는다. 만약 가득 찼으면 아이템을 넣지 못한다.
    /// </summary>
    /// <param name="actor">아이템을 줍는 액터</param>
    public static void PickupAction(Actor actor)
    {
        for (int i = 0; i < GameManager.instance.Entities.Count; i++)
        {
            if (GameManager.instance.Entities[i].GetComponent<Actor>() || 
                actor.transform.position != GameManager.instance.Entities[i].transform.position)
            {
                continue;
            }
            // 인벤토리가 가득 찬 경우 메시지만 출력한다.
            if (actor.Inventory.Items.Count >= actor.Inventory.Capacity)
            {
                UIManager.instance.AddMessage($"당신의 인벤토리는 이미 가득 찼다.", "#808080");
                return;
            }

            Item item = GameManager.instance.Entities[i].GetComponent<Item>();
            actor.Inventory.Add(item);

            UIManager.instance.AddMessage($"당신은 {item.name}을(를) 주웠다.", "#FFFFFF");
            GameManager.instance.EndTurn();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actor"></param>
    public static void TakeStairsAction(Actor actor)
    {
        Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(actor.transform.position);
        string tileName = MapManager.instance.FloorMap.GetTile(pos).name;

        if (tileName != MapManager.instance.UpStairsTile.name && tileName != MapManager.instance.DownStairsTile.name)
        {
            UIManager.instance.AddMessage("이 위치에는 계단이 없다.", "#0da2ff");
            return;
        }

        if (SaveManager.instance.CurrentFloor == 1 && tileName == MapManager.instance.UpStairsTile.name)
        {
            // TODO : 옌더의 아뮬렛을 들고 있는 경우 게임을 클리어 함
            UIManager.instance.AddMessage("신비로운 힘이 당신이 돌아가려는 것을 막는다.", "#0da2ff");
            return;
        }

        SaveManager.instance.SaveGame();
        SaveManager.instance.CurrentFloor += tileName == MapManager.instance.UpStairsTile.name ? -1 : 1;

        if (SaveManager.instance.Save.Scenes.Exists(x => x.FloorNumber == SaveManager.instance.CurrentFloor))
        {
            SaveManager.instance.LoadScene(false);
        }
        else
        {
            GameManager.instance.Reset(false);
            MapManager.instance.GenerateDungeon();
        }

        if (tileName == MapManager.instance.UpStairsTile.name)
        {
            UIManager.instance.AddMessage("당신은 계단을 통해 층을 올라갔다.", "#0da2ff");
        }
        else
        {
            UIManager.instance.AddMessage("당신은 계단을 통해 층을 내려갔다.", "#0da2ff");
        }
        UIManager.instance.SetDungeonFloorText(SaveManager.instance.CurrentFloor);
    }

    /// <summary>
    /// 아이템을 인벤토리에서 버리는 행동
    /// </summary>
    /// <param name="actor">액터</param>
    /// <param name="item">버리려는 아이템</param>
    public static void DropAction(Actor actor, Item item)
    {
        if (actor.Equipment.ItemIsEquipped(item))
        {
            actor.Equipment.ToggleEquip(item);
        }

        actor.Inventory.Drop(item);

        UIManager.instance.ToggleDropMenu();
        GameManager.instance.EndTurn();
    }

    /// <summary>
    /// 아이템을 사용하는 행동
    /// </summary>
    /// <param name="actor">액터</param>
    /// <param name="item">사용하려는 아이템</param>
    public static void UseAction(Actor consumer, Item item)
    {
        bool itemUsed = false;

        if (item.Consumable is not null)
        {
            itemUsed = item.GetComponent<Consumable>().Activate(consumer);
        }

        UIManager.instance.ToggleInventory();

        if (itemUsed)
        { 
            GameManager.instance.EndTurn();
        }
    }

    public static void UpgradeAction(Actor consumer, Item item)
    {

        item.Equippable.Reinforcement += 1;
        UIManager.instance.ToggleSelectMenu(consumer);
        UIManager.instance.AddMessage($"{item.name}에서 빛이 나면서 강한 마법의 기운이 들어왔다.", "#8080ff");
        GameManager.instance.EndTurn();
    }
    /// <summary>
    /// 이동 또는 공격을 하는 행동
    /// </summary>
    /// <param name="actor">기준이 되는 액터</param>
    /// <param name="direction">이동하려는 방향</param>
    /// <returns>이동하려는 방향에 움직임을 막는 다른 엔티티가 있는 경우 false를 반환하고, MeleeAction()을 한다.
    /// 아닌 경우 true를 반환하고 MovementAction을 통해 움직인다.</returns>
    public static bool BumpAction(Actor actor, Vector2 direction)
    {
        Actor target = GameManager.instance.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        if (target)
        {
            MeleeAction(actor, target);
            return false;
        }
        else
        {
            MovementAction(actor, direction);
            return true;
        }
    } 
    /// <summary>
    /// target에 해당하는 엔티티에게 공격을 가한다.
    /// </summary>
    /// <param name="actor">공격을 하는 액터</param>
    /// <param name="target">공격 대상</param>
    public static void MeleeAction(Actor actor, Actor target)
    {
        bool isHit = false; // 맞았는지 여부
        int damage = 0; // 최종 대미지
        // 대미지 설정 - 전투 시스템의 개선을 하려면 여기 부분을 수정한다.
        // damage 변수에 최종 대미지를 계산해서 넣으면 된다.

        // 1. 공격의 명중 여부 설정
        float ACC = (float)actor.GetComponent<Fighter>().Accuracy();
        float EV = (float)target.GetComponent<Fighter>().Evasion();
        float acc_rate = Mathf.Min(( ((65 + ACC * 4.21f) * (110 - EV * 2.40f)) / (100 + 1.015f * EV) ), 100.0f);

        if (Random.Range(0.0f, 100.0f) < acc_rate)
        {
            isHit = true;
        }
        if (isHit)
        {
            // 2. 공격 측 피해량 계산
            int attacker_value = Random.Range(1, // 최소
                                              actor.GetComponent<Fighter>().Power() * 2 + 1); // 최대
            attacker_value = Mathf.FloorToInt((float)attacker_value * (1 + actor.GetComponent<Fighter>().AttackModifier));

            // 3. 방어 측 방어량 계산
            int defender_value = Random.Range(1 + target.GetComponent<Fighter>().Defense() / 4, // 최소
                                              target.GetComponent<Fighter>().Defense() * 2 + 1); // 최대

            // 4. 최종 대미지 계산
            damage = attacker_value - defender_value;
            damage = Mathf.FloorToInt((float)damage * (1 - actor.GetComponent<Fighter>().DefenseModifier));
        }
        // =========================================================================================
        string attackDesc = $"{actor.name}(이)가 {target.name}(을)를 공격";
        
        string colorHex = "";
        // 메시지의 색 설정
        if (actor.GetComponent<Player>())
        {
            colorHex = "#ffffff"; // 흰색
        }
        else
        {
            colorHex = "#d1a3a4"; // 밝은 빨강
        }

        if (!isHit)
        {
            UIManager.instance.AddMessage($"{attackDesc}했으나 빗나갔다.", colorHex);
        }
        else if (damage > 0)
        {
            UIManager.instance.AddMessage($"{attackDesc}해서 {damage}의 피해를 입혔다!", colorHex);
            // 대미지만큼 Hp 감소
            target.GetComponent<Fighter>().Hp -= damage;
        }
        else
        {
            UIManager.instance.AddMessage($"{attackDesc}했으나 피해를 입히지는 못했다.", colorHex);
        }
        // 5. 효과 부여
        // 플레이어 무기 인챈트

        float fireResist = Mathf.Min(target.GetComponent<Fighter>().FireResistance(), 1.0f);
        float poisonResist = Mathf.Min(target.GetComponent<Fighter>().PoisonResistance(), 1.0f);
        // 몬스터
        if (actor.name == "독사" && Random.Range(0.0f, 1.0f) < 0.2f * (1.0f - (poisonResist)))
        {
            target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Poison, Random.Range(3,7));
        }
        else if (actor.name == "벌" && Random.Range(0.0f, 1.0f) < 0.25f * (1.0f - (poisonResist)))
        {
            target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Poison, Random.Range(2,6));
        }
        else if (actor.name == "오우거 마법사" && Random.Range(0.0f, 1.0f) < 0.25f)
        {
            if (Random.Range(0.0f, 1.0f) > 0.6f)
            {
                target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Weakness, 20);
            }
            else if (Random.Range(0.0f, 1.0f) > fireResist)
            {
                target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Burn, Random.Range(4, 8));
            }
        }
        else if (actor.name == "스켈레톤 전사" && Random.Range(0.0f, 1.0f) < 0.2f)
        {
            target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Vulnerability, 20);
        }
        else if (actor.name == "화염 슬라임" && Random.Range(0.0f, 1.0f) < 0.3f * (1 - (fireResist)))
        {
            target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Burn, Random.Range(3, 6));
        }
        else if (actor.name == "화염 박쥐" && Random.Range(0.0f, 1.0f) < 0.3f * (1 - (fireResist)))
        {
            target.GetComponent<Fighter>().AddEffect(Effect.EffectType.Burn, Random.Range(3, 6));
        }
        GameManager.instance.EndTurn();
    }

    /// <summary>
    /// target에 해당하는 엔티티에게 원거리 공격을 가한다.
    /// </summary>
    /// <param name="actor">공격을 하는 액터</param>
    /// <param name="target">공격 대상</param>
    public static void RangeAction(Actor actor, Actor target, int thrownDamage)
    {
        bool isHit = false; // 맞았는지 여부
        int damage = 0; // 최종 대미지
        // 대미지 설정 - 전투 시스템의 개선을 하려면 여기 부분을 수정한다.
        // damage 변수에 최종 대미지를 계산해서 넣으면 된다.

        // 1. 공격의 명중 여부 설정
        float ACC = (float)actor.GetComponent<Fighter>().Accuracy();
        float EV = (float)target.GetComponent<Fighter>().Evasion();
        float acc_rate = Mathf.Min((((65 + ACC * 4.21f) * (110 - EV * 2.40f)) / (100 + 1.015f * EV)), 100.0f);

        if (Random.Range(0.0f, 100.0f) < acc_rate)
        {
            isHit = true;
        }

        if (isHit)
        {
            // 2. 공격 측 피해량 계산
            int attacker_value = thrownDamage;
            attacker_value =
                Mathf.FloorToInt((float)attacker_value * (1 + actor.GetComponent<Fighter>().AttackModifier));

            // 3. 방어 측 방어량 계산
            int defender_value = Random.Range(1 + target.GetComponent<Fighter>().Defense() / 4, // 최소
                target.GetComponent<Fighter>().Defense() * 2 + 1); // 최대

            // 4. 최종 대미지 계산
            damage = attacker_value - defender_value;
            damage = Mathf.FloorToInt((float)damage * (1 - actor.GetComponent<Fighter>().DefenseModifier));
        }

        // =========================================================================================
        string attackDesc = $"{actor.name}(이)가 무기를 던져서 {target.name}(을)를 공격";

        string colorHex = "";
        // 메시지의 색 설정
        if (actor.GetComponent<Player>())
        {
            colorHex = "#ffffff"; // 흰색
        }
        else
        {
            colorHex = "#d1a3a4"; // 밝은 빨강
        }

        if (!isHit)
        {
            UIManager.instance.AddMessage($"{attackDesc}했으나 빗나갔다.", colorHex);
        }
        else if (damage > 0)
        {
            UIManager.instance.AddMessage($"{attackDesc}했고 {damage}의 피해를 입혔다!", colorHex);
            // 대미지만큼 Hp 감소
            target.GetComponent<Fighter>().Hp -= damage;
        }
        else
        {
            UIManager.instance.AddMessage($"{attackDesc}했으나 피해를 입히지는 못했다.", colorHex);
        }
    }

    public static void MovementAction(Actor actor, Vector2 direction)
    {
        // Debug.Log($"{entity.name} moves {direction}!"};
        actor.Move(direction);
        actor.UpdateFieldOfView();
        GameManager.instance.EndTurn();
        if (actor.GetComponent<Player>() && !GameManager.instance.CameraFullMapView)
        {
            Camera.main.transform.position = new Vector3(actor.transform.position.x, actor.transform.position.y, -10);
            Camera.main.orthographicSize = 6;
        }
        
    }

    /// <summary>
    /// 아무것도 하지 않고 턴을 넘기는 행동
    /// </summary>
    public static void WaitAction()
    {
        GameManager.instance.EndTurn();
    }

    /// <summary>
    /// 단일 대상 마법을 시전함
    /// </summary>
    /// <param name="consumer">마법 사용자</param>
    /// <param name="target">마법의 타겟(단일)</param>
    /// <param name="consumable">사용하는 마법 아이템</param>
    public static void CastAction(Actor consumer, Actor target, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, target);

        if (castSuccess)
        {
            GameManager.instance.EndTurn();
        }
    }

    /// <summary>
    /// 광역기 마법을 시전함
    /// </summary>
    /// <param name="consumer">마법 사용자</param>
    /// <param name="targets">마법의 타겟(다중)</param>
    /// <param name="consumable">사용하는 마법 아이템</param>
    public static void CastAction(Actor consumer, List<Actor> targets, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, targets);

        if (castSuccess)
        {
            GameManager.instance.EndTurn();
        }
    }

    /// <summary>
    /// 아이템을 장착하는 행동
    /// </summary>
    /// <param name="actor">장착하는 액터(대개 플레이어)</param>
    /// <param name="item">장착하는 아이템</param>
    public static void EquipAction(Actor actor, Item item)
    {
        if (item.Equippable is null)
        {
            UIManager.instance.AddMessage($"{item.name}은(는) 장착할 수 없다.", "#808080");
            return;
        }

        actor.Equipment.ToggleEquip(item);

        UIManager.instance.ToggleInventory();
        GameManager.instance.EndTurn();
    }
}

using System.Collections.Generic;
using UnityEngine;

// 전투 시스템의 핵심인 체력, 공격력 등을 다루는 곳, 게임을 흥미롭게 만들려면 이곳을 수정하는 것이 필수이다.
[RequireComponent(typeof(Actor), typeof(Effect))]
public class Fighter : MonoBehaviour
{
    [SerializeField] private int maxHp, hp, baseDefense, basePower; // 기본적인 스탯
    [SerializeField] private int baseAccuracy, baseEvasion; // 명중률, 회피율
    [SerializeField] private Actor target; // 공격할 대상
    [SerializeField] private List<Effect> effects = new List<Effect>();
    [SerializeField] private float defenseModifier = 0.0f, attackModifier = 0.0f;

    public int Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Max(0, Mathf.Min(maxHp, value));
            // 플레이어의 경우 체력바 UI를 업데이트 함
            if (GetComponent<Player>())
            {
                UIManager.instance.SetHealth(hp, maxHp);
            }
            if (hp <= 0)
                Die();
        }
    }

    public int MaxHp {
        get => maxHp;
        set {
            maxHp = value;
            if (GetComponent<Player>())
            {
                UIManager.instance.SetHealthMax(maxHp);
            }
        }  
    }
    public int BaseDefense { get => baseDefense; set => baseDefense = value; }
    public int BasePower { get => basePower; set => basePower = value; }
    public Actor Target { get => target; set => target = value;  }

    public int BaseAccuracy { get => baseAccuracy; set => baseAccuracy = value; }
    public int BaseEvasion { get => baseEvasion; set => baseEvasion = value; }

    public float AttackModifier { get => attackModifier; set => attackModifier = value; }
    public float DefenseModifier { get => defenseModifier; set => defenseModifier = value; }
    public List<Effect> Effects { get => effects; set => effects = value; }

    // 속성 값을 반환
    public int Power()
    {
        return basePower + PowerBonus();
    }

    public int Defense()
    {
        return baseDefense + DefenseBonus();
    }

    public int Evasion()
    {
        return baseEvasion + EvasionBonus();
    }

    public int Accuracy()
    {
        return baseAccuracy + AccuracyBonus();
    }

    public int PowerBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().PowerBonus();
        }
        return 0;
    }

    public int DefenseBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().DefenseBonus();
        }
        return 0;
    }

    public int EvasionBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().EvasionBonus();
        }

        return 0;
    }

    public int AccuracyBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().AccuracyBonus();
        }
        return 0;
    }

    private void Start()
    {
        if (GetComponent<Player>())
        {
            UIManager.instance.SetHealthMax(maxHp);
            UIManager.instance.SetHealth(hp, maxHp);
        }
    }
    /// <summary>
    /// 해당 액터를 시체로 바꾼다.
    /// </summary>
    public void Die()
    {
        if (GetComponent<Actor>().IsAlive) 
        { 
            if (GetComponent<Player>())
            {
                UIManager.instance.AddMessage($"당신은 쓰러졌다...", "#ff0000"); // 빨강
                // 세이브 파일 삭제
                SaveManager.instance.DeleteSave();
            }
            else
            {
                GameManager.instance.Actors[0].GetComponent<Level>().AddExperience(GetComponent<Level>().XpGiven); // 플레이어에게 Xp를 제공
                UIManager.instance.AddMessage($"{name}이(가) 쓰러졌다.", "#ffa500"); // 밝은 주황
            }
            GetComponent<Actor>().IsAlive = false;
        }
        // 걸려있는 모든 효과 제거
        effects.Clear();
        // 시체 스프라이트로 변경
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.DeadSprite;
        spriteRenderer.color = new Color(191, 0, 0, 1);
        spriteRenderer.sortingOrder = 0;

        name = $"{name}의 시체";
        GetComponent<Actor>().BlocksMovement = false;

        // 리스트에서 해당 액터 제거
        if (!GetComponent<Player>())
        {
            GameManager.instance.RemoveActor(this.GetComponent<Actor>());
        }
    }

    /// <summary>
    /// 액터의 체력을 회복시키고 회복량을 반환한다.
    /// 최대 체력을 초과하게 회복시키지는 않는다.
    /// 회복하려는 양을 더했을 때 최대 체력을 초과하는 경우 amount보다 작은 수가 반환될 수 있다.
    /// </summary>
    /// <param name="amount">회복하려는 양</param>
    /// <returns>실제로 회복에 적용된 수치를 int로 반환한다.</returns>
    public int Heal(int amount)
    {
        if (hp == maxHp)
        {
            return 0;
        }

        int newHPValue = hp + amount;

        if (newHPValue > maxHp)
        {
            newHPValue = maxHp;
        }

        int amountRecovered = newHPValue - hp;
        Hp = newHPValue;
        return amountRecovered;
    }

    public void AddEffect(Effect.EffectType type, int turns)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            // 기존 효과와 중복된 경우 그 효과의 시간을 연장함
            if (effects[i]._EffectType == type)
            {
                effects[i].LeftTurn += turns;
                UIManager.instance.AddMessage($"{name}에게 걸려 있던 {effects[i].EffectName()} 효과가 {turns}턴 연장되었다.", "#ff8000");
                return;
            }
        }
        Effect effect = new Effect();
        effect._EffectType = type;
        effect.LeftTurn = turns;

        UIManager.instance.AddMessage($"{name}에게 {effect.EffectName()} 효과가 {turns}턴 부여되었다.", "#ff8000");
        effects.Add(effect);
    }

    public void EffectCheck()
    {
        attackModifier = 0;
        defenseModifier = 0;
        for (int i = 0; i < effects.Count; i++)
        {
            // 턴 수를 하나 낮춤
            effects[i].LeftTurn -= 1;
            // 만약 지속시간이 끝난 경우 제거
            if (effects[i].LeftTurn < 0)
            {
                UIManager.instance.AddMessage($"{name}의 {effects[i].EffectName()} 효과가 끝났다.", "#FFFF00"); // 노랑
                switch (effects[i]._EffectType)
                {
                    case Effect.EffectType.Poison:
                        break;
                    case Effect.EffectType.Burn:
                        break;
                    case Effect.EffectType.Paralysis:
                        break;
                    case Effect.EffectType.Regeneration:
                        break;
                    case Effect.EffectType.Rage:
                        attackModifier = 0;
                        break;
                    case Effect.EffectType.Protection:
                        defenseModifier = 0;
                        break;
                    case Effect.EffectType.MagicalVision:
                        this.GetComponent<Actor>().FieldOfViewRange = 8;
                        break;
                    case Effect.EffectType.Weakness:
                        attackModifier = 0;
                        break;
                    case Effect.EffectType.Vulnerability:
                        defenseModifier = 0;
                        break;

                }
                effects.Remove(effects[i]);
            }
            else
            {
                // 효과 발동
                switch (effects[i]._EffectType)
                {
                    case Effect.EffectType.Poison:
                        Hp -= (effects[i].LeftTurn / 3) + 1;
                        UIManager.instance.AddMessage($"{name}은(는) 독에 의해 {(effects[i].LeftTurn / 3) + 1}대미지를 입었다.", "#ff8000"); // 주황
                        break;
                    case Effect.EffectType.Burn:
                        int damage = Random.Range(1, (effects[i].LeftTurn / 2) + 1);
                        Hp -= damage;
                        UIManager.instance.AddMessage($"{name}은(는) 불에 의해 {damage}대미지를 입었다.", "#ff8000"); // 주황
                        break;
                    case Effect.EffectType.Paralysis:
                        break;
                    case Effect.EffectType.Regeneration:
                        Hp += (effects[i].LeftTurn / 2) + 1;
                        break;
                    case Effect.EffectType.Rage:
                        attackModifier += 0.4f;
                        break;
                    case Effect.EffectType.Protection:
                        defenseModifier += 0.4f;
                        break;
                    case Effect.EffectType.MagicalVision:
                        this.GetComponent<Actor>().FieldOfViewRange = 20;
                        break;
                    case Effect.EffectType.Weakness:
                        attackModifier -= 0.4f;
                        break;
                    case Effect.EffectType.Vulnerability:
                        defenseModifier -= 0.4f;
                        break;

                }
            }
        }
    }

    public FighterState SaveState() => new FighterState(
        maxHp : maxHp,
        hp : hp,
        baseDefense : baseDefense,
        basePower : basePower,
        target : target != null ? target.name : null,
        baseAccuracy : baseAccuracy,
        baseEvasion :baseEvasion,
        attackModifier : attackModifier,
        defenseModifier : defenseModifier,
        effects : effects
    );

    public void LoadState(FighterState state)
    {
        maxHp = state.MaxHp;
        hp = state.Hp;
        baseDefense = state.BaseDefense;
        basePower = state.BasePower;
        target = GameManager.instance.Actors.Find(a => a.name == state.Target);
        baseAccuracy = state.BaseAccuracy;
        baseEvasion = state.BaseEvasion;
        attackModifier = state.AttackModifier;
        defenseModifier = state.DefenseModifier;
        effects = state.Effects;
    }
}

public class FighterState{
    [SerializeField] private int maxHp, hp, baseDefense, basePower;
    [SerializeField] private int baseAccuracy, baseEvasion;
    [SerializeField] private string target;
    [SerializeField] private List<Effect> effects = new List<Effect>();
    [SerializeField] private float defenseModifier = 0.0f, attackModifier = 0.0f;

    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Hp { get => hp; set => hp = value; }
    public int BaseDefense { get => baseDefense; set => baseDefense = value; }
    public int BasePower { get => basePower; set => basePower = value; }
    public int BaseAccuracy { get => baseAccuracy; set => baseAccuracy = value; }
    public int BaseEvasion { get => baseEvasion; set => baseEvasion = value; }
    public float AttackModifier { get => attackModifier; set => attackModifier = value; }
    public float DefenseModifier { get => defenseModifier; set => defenseModifier = value; }
    public string Target { get => target; set => target = value; }
    public List<Effect> Effects { get => effects; set => effects = value; }

    public FighterState(int maxHp, int hp, int baseDefense, int basePower, string target,
        int baseAccuracy, int baseEvasion, float attackModifier, float defenseModifier, List<Effect> effects) 
    { 
        this.maxHp = maxHp;
        this.hp = hp;
        this.baseDefense = baseDefense;
        this.basePower = basePower;
        this.target = target;
        this.baseAccuracy = baseAccuracy;
        this.baseEvasion = baseEvasion;
        this.attackModifier = attackModifier;
        this.defenseModifier = defenseModifier;
        this.effects = effects;
    }
}

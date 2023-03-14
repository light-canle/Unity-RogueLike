using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Level : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel, levelUpBase = 200, levelUpFactor = 150, xpGiven;

    public int CurrentLevel { get => currentLevel; }
    public int CurrentXp { get => currentXp; }
    public int XpToNextLevel { get => xpToNextLevel; }
    public int XpGiven { get => xpGiven; set => xpGiven = value; }

    private void OnValidate() => xpToNextLevel = ExperienceToNextLevel();

    private int ExperienceToNextLevel() => levelUpBase + currentLevel * levelUpFactor;

    private bool RequiresLevelUp() => currentXp >= xpToNextLevel;

    public void AddExperience(int xp)
    {
        if (xp == 0 || levelUpBase == 0) return;

        currentXp += xp;

        UIManager.instance.AddMessage($"당신은 {xp} 경험치를 획득했다.","#FFFFFF");

        if (RequiresLevelUp())
        {
            UIManager.instance.ToggleLevelUpMenu(GetComponent<Actor>());
            UIManager.instance.AddMessage($"당신의 레벨이 {currentLevel + 1}(으)로 올랐다!", "#00FF00"); // 초록색
        }
    }

    private void IncreaseLevel()
    {
        currentXp -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = ExperienceToNextLevel();
    }

    public void IncreaseMaxHp(int amount = 20)
    {
        GetComponent<Fighter>().MaxHp += amount;
        GetComponent<Fighter>().Hp += amount;

        UIManager.instance.AddMessage($"당신의 생명력이 늘어났다!", "#00ff00");
        IncreaseLevel();
    }

    public void IncreasePower(int amount = 1)
    {
        GetComponent<Fighter>().BasePower += amount;

        UIManager.instance.AddMessage($"당신의 힘이 강해졌다!", "#00ff00");
        IncreaseLevel();
    }
    
    public void IncreaseDefense(int amount = 1)
    {
        GetComponent<Fighter>().BaseDefense += amount;

        UIManager.instance.AddMessage($"당신 주변의 마법 보호막이 강해졌다!", "#00ff00");
        IncreaseLevel();
    }

    public void IncreaseEvasion(int amount = 1)
    {
        GetComponent<Fighter>().BaseEvasion += amount;

        UIManager.instance.AddMessage($"당신의 움직임이 더 빨라졌다!", "#00ff00");
        IncreaseLevel();
    }

    public void IncreaseAccuracy(int amount = 1)
    {
        GetComponent<Fighter>().BaseAccuracy += amount;

        UIManager.instance.AddMessage($"당신의 공격은 더 정확해졌다!", "#00ff00");
        IncreaseLevel();
    }

    public LevelState SaveState() => new LevelState(
        currentLevel: currentLevel,
        currentXp: currentXp,
        xpToNextLevel: xpToNextLevel
    );

    public void LoadState(LevelState state)
    {
        currentLevel = state.CurrentLevel;
        currentXp = state.CurrentXp;
        xpToNextLevel = state.XpToNextLevel;
    }
}

public class LevelState
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CurrentXp { get => currentXp; set => currentXp = value; }
    public int XpToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

    public LevelState(int currentLevel, int currentXp, int xpToNextLevel)
    {
        this.currentLevel = currentLevel;
        this.currentXp = currentXp;
        this.xpToNextLevel = xpToNextLevel;
    }
}

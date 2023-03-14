using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private TextMeshProUGUI dungeonFloorText;
    [SerializeField] private TextMeshProUGUI playerEffectListText;

    [Header("Health UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;

    [Header("Message UI")]
    [SerializeField] private int sameMessageCount = 0; // Read-Only
    [SerializeField] private string lastMessage; // Read-Only
    [SerializeField] private bool isMessageHistoryOpen = false; // Read-Only
    [SerializeField] private GameObject messageHistory;
    [SerializeField] private GameObject messageHistoryContent;
    [SerializeField] private GameObject lastFiveMessageContent;

    [Header("Inventory UI")]
    [SerializeField] private bool isInventoryOpen = false; // Read-Only
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryContent;

    [Header("Drop Menu UI")]
    [SerializeField] private bool isDropMenuOpen = false; // Read-Only
    [SerializeField] private GameObject dropMenu;
    [SerializeField] private GameObject dropMenuContent;

    [Header("Select Menu UI")]
    [SerializeField] private bool isSelectMenuOpen = false; // Read-Only
    [SerializeField] private GameObject selectMenu;
    [SerializeField] private GameObject selectMenuContent;

    [Header("Escape Menu UI")]
    [SerializeField] private bool isEscapeMenuOpen = false; // Read-Only
    [SerializeField] private GameObject escapeMenu;

    [Header("Character Information Menu UI")]
    [SerializeField] private bool isCharacterInformationMenuOpen = false; // Read-Only
    [SerializeField] private GameObject characterInformationMenu;

    [Header("Level Up Menu UI")]
    [SerializeField] private bool isLevelUpMenuOpen = false; // Read-Only
    [SerializeField] private GameObject levelUpMenu;
    [SerializeField] private GameObject levelUpMenuContent;

    public bool IsMenuOpen { get => isMenuOpen; }
    public bool IsMessageHistoryOpen { get => isMessageHistoryOpen; }
    public bool IsInventoryOpen { get => isInventoryOpen; }
    public bool IsDropMenuOpen { get => isDropMenuOpen; }
    public bool IsEscapeMenuOpen { get => isEscapeMenuOpen; }
    public bool IsCharacterInformationMenuOpen { get => isCharacterInformationMenuOpen; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        SetDungeonFloorText(SaveManager.instance.CurrentFloor);
        if (SaveManager.instance.Save.SavedFloor is 0)
        {
            AddMessage("랜덤 던전에 오신 것을 환영합니다!", "#0da2ff"); // 밝은 파랑
        }
        else
        {
            AddMessage("다시 오신 것을 환영합니다!", "#0da2ff"); // 밝은 파랑
        }
    }

    /// <summary>
    /// 체력 슬라이더 UI에 표시할 최대 체력을 설정한다.
    /// </summary>
    /// <param name="maxHp">최대 체력</param>
    public void SetHealthMax(int maxHp)
    {
        hpSlider.maxValue = maxHp;
    }
    /// <summary>
    /// 현재 체력을 받아 슬라이더 길이와 텍스트를 설정한다.
    /// </summary>
    /// <param name="hp">현재 체력</param>
    /// <param name="maxHp">최대 체력</param>
    public void SetHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHp}";
    }
    /// <summary>
    /// 현재 층수를 받아 텍스트를 설정한다.
    /// </summary>
    /// <param name="floor">현재 던전의 층수</param>
    public void SetDungeonFloorText(int floor)
    {
        dungeonFloorText.text = $"층 수 : {floor}";
    }
    /// <summary>
    /// 플레이어가 걸린 효과들을 출력한다.
    /// </summary>
    /// <param name="effects">효과들을 담은 리스트</param>
    public void SetPlayerEffectListText(List<Effect> effects)
    {
        string effectText = "";
        
        for (int i = 0; i < effects.Count; i++)
        {
            effectText += effects[i].EffectName() + " : " + effects[i].LeftTurn.ToString() + "\n";
        }
        playerEffectListText.text = effectText;
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            switch (true)
            {
                case bool _ when isMessageHistoryOpen:
                    ToggleMessageHistory();
                    break;
                case bool _ when isInventoryOpen:
                    ToggleInventory();
                    break;
                case bool _ when isDropMenuOpen:
                    ToggleDropMenu();
                    break;
                case bool _ when isEscapeMenuOpen:
                    ToggleEscapeMenu();
                    break;
                case bool _ when isCharacterInformationMenuOpen:
                    ToggleCharacterInformationMenu();
                    break;
                case bool _ when isSelectMenuOpen:
                    isMenuOpen = true;
                    ToggleSelectMenu();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 과거 기록을 표시하는 UI를 화면에 나타내거나 가린다.
    /// </summary>
    public void ToggleMessageHistory()
    {
        isMessageHistoryOpen = !isMessageHistoryOpen;
        SetBooleans(messageHistory, isMessageHistoryOpen);
    }

    public void ToggleInventory(Actor actor = null)
    {
        isInventoryOpen = !isInventoryOpen;
        SetBooleans(inventory, isInventoryOpen);

        if (isMenuOpen)
        {
            UpdateMenu(actor, inventoryContent);
        }
    }

    public void ToggleDropMenu(Actor actor = null)
    {
        isDropMenuOpen = !isDropMenuOpen;
        SetBooleans(dropMenu, isDropMenuOpen);

        if (isMenuOpen)
        {
            UpdateMenu(actor, dropMenuContent);
        }
    }

    // type의 0은 강화의 주문서 / 1은 마법 부여 책
    public void ToggleSelectMenu(Actor actor = null, int type = 0)
    {
        //UIManager.instance.ToggleInventory();
        isSelectMenuOpen = !isSelectMenuOpen;
        SetBooleans(selectMenu, isSelectMenuOpen);

        if (isMenuOpen)
        {
            UpdateMenu(actor, selectMenuContent);
        }
    }

    public void ToggleEscapeMenu()
    {
        isEscapeMenuOpen = !isEscapeMenuOpen;
        SetBooleans(escapeMenu, isEscapeMenuOpen);

        eventSystem.SetSelectedGameObject(escapeMenu.transform.GetChild(0).gameObject);
    }

    public void ToggleLevelUpMenu(Actor actor)
    {
        isLevelUpMenuOpen = !isLevelUpMenuOpen;
        SetBooleans(levelUpMenu, isLevelUpMenuOpen);

        GameObject constitutionButton = levelUpMenuContent.transform.GetChild(0).gameObject;
        GameObject strengthButton = levelUpMenuContent.transform.GetChild(1).gameObject;
        GameObject agilityButton = levelUpMenuContent.transform.GetChild(2).gameObject;

        constitutionButton.GetComponent<TextMeshProUGUI>().text = $"a) 건강 (+20 체력 | 현재:{actor.GetComponent<Fighter>().MaxHp})";
        strengthButton.GetComponent<TextMeshProUGUI>().text = $"b) 힘 (+ 1 힘 | 현재:{actor.GetComponent<Fighter>().Power()} )";
        agilityButton.GetComponent<TextMeshProUGUI>().text = $"c) 보호막 (+ 1 방어 | 현재:{actor.GetComponent<Fighter>().Defense()})";

        foreach (Transform child in levelUpMenuContent.transform)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();

            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (constitutionButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreaseMaxHp();
                }
                else if (strengthButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreasePower();
                }
                else if (agilityButton == child.gameObject)
                {
                    actor.GetComponent<Level>().IncreaseDefense();
                }
                else
                {
                    Debug.LogError("레벨 업 UI - 버튼을 찾을 수 없습니다.");
                }
                ToggleLevelUpMenu(actor);
            });
        }

        eventSystem.SetSelectedGameObject(levelUpMenuContent.transform.GetChild(0).gameObject);
    }

    public void ToggleCharacterInformationMenu(Actor actor = null)
    {
        isCharacterInformationMenuOpen = !isCharacterInformationMenuOpen;
        SetBooleans(characterInformationMenu, isCharacterInformationMenuOpen);

        if (actor is not null)
        {
            characterInformationMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"레벨 : {actor.GetComponent<Level>().CurrentLevel}";
            characterInformationMenu.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                $"경험치 : {actor.GetComponent<Level>().CurrentXp}";
            characterInformationMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                $"다음 레벨 요구 경험치 : {actor.GetComponent<Level>().XpToNextLevel}";
            characterInformationMenu.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                $"힘 : {actor.GetComponent<Fighter>().Power()}";
            characterInformationMenu.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
                $"방어력 : {actor.GetComponent<Fighter>().Defense()}";
            characterInformationMenu.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
                $"명중 : {actor.GetComponent<Fighter>().Accuracy()}";
            characterInformationMenu.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text =
                $"회피 : {actor.GetComponent<Fighter>().Evasion()}";
        }
    }

    private void SetBooleans(GameObject menu, bool menuBool)
    {
        isMenuOpen = menuBool;
        menu.SetActive(menuBool);
    }

    public void Save()
    {
        // 플레이어가 살아 있는 경우에만 저장
        //AddMessage("저장 중 입니다.", "#0da2ff");
        SaveManager.instance.SaveGame(false);
        StartCoroutine(WaitTime(0.5f));
        //AddMessage("저장 완료", "#0da2ff");
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void Load()
    {
        SaveManager.instance.LoadGame();
        AddMessage("당신이 가장 마지막에 저장한 시간으로 돌아갑니다.", "#0da2ff");
        ToggleMenu();
    }

    public void Quit()
    {
        foreach(Actor actor in GameManager.instance.Actors)
        {
            if (actor.GetComponent<Player>() && actor.IsAlive)
            {
                Save();
            }
        }
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// newMessage를 colorHex의 색깔로 출력한다.
    /// </summary>
    /// <param name="newMessage"></param>
    /// <param name="colorHex"></param>
    public void AddMessage(string newMessage, string colorHex)
    {
        // 만약 가장 최근의 메시지와 겹치는 경우 가장 최근 텍스트 옆에 (x2) 같은 식으로 중첩되었음을 표시
        if (lastMessage == newMessage)
        {
            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lastFiveHistoryLastChild = lastFiveMessageContent.transform.GetChild(lastFiveMessageContent.transform.childCount -1).GetComponent<TextMeshProUGUI>();
            messageHistoryLastChild.text = $"{newMessage} (x{++sameMessageCount})";
            lastFiveHistoryLastChild.text = $"{newMessage} (x{sameMessageCount})";
            return;
        }
        // 중복 메시지가 아닌 경우 다시 sameMessageCount를 0으로 설정
        else if (sameMessageCount > 0)
        {
            sameMessageCount = 0;
        }
        // 새 메시지를 최근 메시지로 설정
        lastMessage = newMessage;
        sameMessageCount++;
        // 메시지 내용과 색 설정
        TextMeshProUGUI messagePrefab = Instantiate(Resources.Load<TextMeshProUGUI>("Message")) as TextMeshProUGUI;
        messagePrefab.text = newMessage;
        messagePrefab.color = GetColorFromHex(colorHex);
        messagePrefab.transform.SetParent(messageHistoryContent.transform, false);

        // 메시지를 최근 5개 메시지를 표시하는 곳과 전체 메시지를 표시하는 곳에 넣음
        for (int i = 0; i < lastFiveMessageContent.transform.childCount; i++)
        {
            if (messageHistoryContent.transform.childCount - 1 < i)
            {
                return;
            }

            TextMeshProUGUI lastFiveHistoryChild = lastFiveMessageContent.transform.GetChild(lastFiveMessageContent.transform.childCount - i - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI messageHistoryChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - i - 1).GetComponent<TextMeshProUGUI>();
            lastFiveHistoryChild.text = messageHistoryChild.text;
            lastFiveHistoryChild.color = messageHistoryChild.color;
        }

    }

    /// <summary>
    /// Hex Code (예:#123456)에 대응하는 색을 반환한다.
    /// 만약 잘못된 값을 입력할 경우 흰색을 반환한다.
    /// </summary>
    /// <param name="v">Hex Code</param>
    /// <returns></returns>
    private Color GetColorFromHex(string v)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(v, out color))
        {
            return color;
        }
        else
        {
            Debug.Log("GetColorFromHex: Could not parse color from string.");
            return Color.white;
        }
    }

    private void UpdateMenu(Actor actor, GameObject menuContent)
    {
        for (int resetNum = 0; resetNum < menuContent.transform.childCount; resetNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(resetNum).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            menuContentChild.GetComponent<Button>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        char c = 'a';

        for (int itemNum = 0; itemNum < actor.Inventory.Items.Count; itemNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(itemNum).gameObject;
            Item item = actor.Inventory.Items[itemNum];
            if (item.Equippable is not null)
            {
                menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) (+{item.Equippable.Reinforcement}) {item.name}";
            }
            else
            {
                menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) {item.name}";
            }
            menuContentChild.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (menuContent == inventoryContent)
                {
                    if (item.Consumable is not null)
                    {
                        Action.UseAction(actor, item);
                    }
                    else if (item.Equippable is not null)
                    {
                        Action.EquipAction(actor, item);
                    }
                }
                else if (menuContent == dropMenuContent)
                {
                    Action.DropAction(actor, item);
                }
                else if (menuContent == selectMenuContent)
                {
                    if (item.Equippable is not null)
                    {
                        Action.UpgradeAction(actor, item);
                    }
                    else
                    {
                        AddMessage("이 아이템은 업그레이드 할 수 없는 것 같습니다.", "#CFCFCF");
                    }
                    
                }
                UpdateMenu(actor, menuContent);
            });
            menuContentChild.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);
    }
}

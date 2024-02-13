using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    [SerializeField] private bool moveKeyDown;
    [SerializeField] private bool targetMode; // read-only
    [SerializeField] private bool isSingleTarget; // read-only
    [SerializeField] private GameObject targetObject;

    private void Awake() => controls = new Controls();

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Player.Disable();
    }

    void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (context.started && GetComponent<Actor>().IsAlive)
        {
            if (targetMode && !moveKeyDown)
            {
                moveKeyDown = true;
                Move();
            }
            else if (!targetMode)
            {
                moveKeyDown = true;
            }

        }
        else if (context.canceled)
        {
            moveKeyDown = false;
        }
    }

    void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetMode)
            {
                ToggleTargetMode();
            }
            else if (!UIManager.instance.IsEscapeMenuOpen && !UIManager.instance.IsMenuOpen)
            {
                UIManager.instance.ToggleEscapeMenu();
            }
            else if (UIManager.instance.IsMenuOpen)
            {
                UIManager.instance.ToggleMenu();
            }
        }
    }


    public void OnView(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (!UIManager.instance.IsMenuOpen || UIManager.instance.IsMessageHistoryOpen)
                UIManager.instance.ToggleMessageHistory();
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct())
            {
                Action.PickupAction(GetComponent<Actor>());
            }
        }
    }

    public void OnRest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Action.WaitAction();
        }
    }
    /// <summary>
    /// 플레이어의 인벤토리를 연다.
    /// </summary>
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsInventoryOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleInventory(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("당신은 아무 아이템도 가지고 있지 않다.", "#808080");
                }
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsDropMenuOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleDropMenu(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("당신은 아무 아이템도 가지고 있지 않다.", "#808080");
                }
            }
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetMode)
            {
                if (isSingleTarget)
                {
                    Actor target = SingleTargetChecks(targetObject.transform.position);

                    if (target != null)
                    {
                        Action.CastAction(GetComponent<Actor>(), target, GetComponent<Inventory>().SelectedConsumable);
                    }
                }
                else
                {
                    List<Actor> targets = AreaTargetChecks(targetObject.transform.position);

                    if (targets != null)
                    {
                        Action.CastAction(GetComponent<Actor>(), targets, GetComponent<Inventory>().SelectedConsumable);
                    }
                }
            }
            else if (CanAct())
            {
                Action.TakeStairsAction(GetComponent<Actor>());
            }
        }
    }

    public void OnInfo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsCharacterInformationMenuOpen)
            {
                UIManager.instance.ToggleCharacterInformationMenu(GetComponent<Actor>());
            }
        }
    }

    public void OnChangeCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.CameraFullMapView = !GameManager.instance.CameraFullMapView;
            // 전체 뷰 - 던전 전체를 보여줌
            if (GameManager.instance.CameraFullMapView)
            {
                Camera.main.transform.position = new Vector3(40, 20.25f, -10);
                Camera.main.orthographicSize = 27;
            }
            // 플레이어 주변을 확대해 보여줌
            else
            {
                Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10);
                Camera.main.orthographicSize = 6;
            }
        }

    }

    public void ChangeCamera()
    {
        GameManager.instance.CameraFullMapView = !GameManager.instance.CameraFullMapView;
        // 전체 뷰 - 던전 전체를 보여줌
        if (GameManager.instance.CameraFullMapView)
        {
            Camera.main.transform.position = new Vector3(40, 20.25f, -10);
            Camera.main.orthographicSize = 27;
        }
        // 플레이어 주변을 확대해 보여줌
        else
        {
            var o = gameObject;
            Camera.main.transform.position = new Vector3(o.transform.position.x, o.transform.position.y, -10);
            Camera.main.orthographicSize = 6;
        }
    }

    /// <summary>
    /// 마법을 시전하려는 장소 또는 타겟을 정한다.
    /// </summary>
    /// <param name="isArea">범위 공격인지 여부 (default = false)</param>
    /// <param name="radius">공격 범위 (단일 공격인 경우 radius = 1)</param>
    public void ToggleTargetMode(bool isArea = false, int radius = 1)
    {
        targetMode = !targetMode;

        if (targetMode)
        {
            if (targetObject.transform.position != transform.position)
            {
                targetObject.transform.position = transform.position;
            }

            if (isArea)
            {
                isSingleTarget = false;
                targetObject.transform.GetChild(0).localScale = Vector3.one * (radius + 1); // +1 to account for the center
                targetObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                isSingleTarget = true;
            }

            targetObject.SetActive(true);
        }
        else
        {
            if (targetObject.transform.GetChild(0).gameObject.activeSelf)
            {
                targetObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            targetObject.SetActive(false);
            GetComponent<Inventory>().SelectedConsumable = null;
        }
    }

    private void FixedUpdate()
    {
        // 메뉴 창이 떠있으면 움직이지 않음
        if (!UIManager.instance.IsMenuOpen && !targetMode && !UIManager.instance.IsSelectMenuOpen)
        {
            if (GameManager.instance.IsPlayerTurn && moveKeyDown && GetComponent<Actor>().IsAlive)
                Move();
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Vector3 futurePosition; // 이동하려는 위치
        if (targetMode) // 마법 시전을 위한 타겟을 움직일 때 - 플레이어가 아닌 타겟의 위치를 옮김
        {
            futurePosition = targetObject.transform.position + (Vector3)roundedDirection;
        }
        else
        {
            futurePosition = transform.position + (Vector3)roundedDirection;
        }

        if (targetMode) // 마법 시전을 위한 타겟을 움직일 때 - 플레이어가 아닌 타겟의 위치를 옮김
        {
            Vector3Int targetGridPosition = MapManager.instance.FloorMap.WorldToCell(futurePosition);
            // 플레이어의 시야 범위 내에서만 쓸 수 있도록 함
            if (MapManager.instance.IsValidPosition(futurePosition) &&
                GetComponent<Actor>().FieldOfView.Contains(targetGridPosition))
            {
                targetObject.transform.position = futurePosition;
            }
        }
        else
        {
            moveKeyDown = Action.BumpAction(GetComponent<Actor>(), roundedDirection); // If we bump into an entity, moveKeyDown is set to False
        }
    }

    private bool CanAct()
    {
        if (targetMode || UIManager.instance.IsMenuOpen || 
            UIManager.instance.IsSelectMenuOpen || !GetComponent<Actor>().IsAlive)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Actor SingleTargetChecks(Vector3 targetPosition)
    {
        Actor target = GameManager.instance.GetActorAtLocation(targetPosition);
        // 액터인 타겟을 선택하지 않음
        if (target == null)
        {
            UIManager.instance.AddMessage("당신은 타겟으로 반드시 적을 선택해야 한다.", "#ffffff");
            return null;
        }
        // 자신을 타겟팅함
        if (target == GetComponent<Actor>())
        {
            UIManager.instance.AddMessage("당신은 스스로를 타겟으로 삼을 수 없다.", "#ffffff");
            return null;
        }

        return target;
    }

    private List<Actor> AreaTargetChecks(Vector3 targetPosition)
    {
        // Take away 1 to account for the center
        int radius = (int)targetObject.transform.GetChild(0).localScale.x - 1;

        // 지정한 위치를 기준으로 하는 가상의 원 범위 생성
        Bounds targetBounds = new Bounds(targetPosition, Vector3.one * radius * 2);
        List<Actor> targets = new List<Actor>();

        foreach (Actor target in GameManager.instance.Actors)
        {
            // 해당 범위 내에 속한 적들을 모두 리스트에 추가
            if (targetBounds.Contains(target.transform.position))
            {
                targets.Add(target);
            }
        }

        if (targets.Count == 0)
        {
            UIManager.instance.AddMessage("해당 범위 안에 타겟이 존재하지 않습니다.", "#ffffff");
            return null;
        }

        return targets;
    }
}
